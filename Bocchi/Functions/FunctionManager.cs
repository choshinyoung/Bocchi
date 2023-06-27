using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bocchi.Functions.Attributes;
using OpenAI.ObjectModels.RequestModels;

namespace Bocchi.Functions;

public class FunctionManager
{
    public List<FunctionInfo> Functions = new();

    public void LoadFunctions()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var types = FilterAssemblyTypes(assembly);

        LoadFunctionsFromTypes(types);
    }

    private static IEnumerable<TypeInfo> FilterAssemblyTypes(Assembly assembly)
    {
        return assembly.DefinedTypes
            .Where(type => type.IsPublic || type.IsNestedPublic)
            .Where(
                type => typeof(FunctionModuleBase<FunctionContext>).IsAssignableFrom(type)
                        && type is { IsAbstract: false, ContainsGenericParameters: false }
            )
            .ToList();
    }

    private void LoadFunctionsFromTypes(IEnumerable<TypeInfo> types)
    {
        foreach (var method in types.SelectMany(type => type.GetMethods()))
        {
            var functionAttribute =
                method.GetCustomAttributes(typeof(FunctionAttribute)).FirstOrDefault() as FunctionAttribute;

            if (!method.IsPublic || functionAttribute is null)
            {
                continue;
            }

            LoadFunction(method, functionAttribute);
        }
    }

    private void LoadFunction(MethodInfo method, FunctionAttribute functionAttribute)
    {
        var functionBuilder = new FunctionDefinitionBuilder(functionAttribute.Name, functionAttribute.Description);

        foreach (var parameter in method.GetParameters())
        {
            var enumAttribute =
                parameter.GetCustomAttributes(typeof(EnumValueAttribute)).FirstOrDefault() as EnumValueAttribute;
            var paramAttribute =
                parameter.GetCustomAttributes(typeof(ParamAttribute)).FirstOrDefault() as ParamAttribute;

            functionBuilder.AddParameter(
                parameter.Name!,
                FindType(parameter.ParameterType),
                paramAttribute?.Description,
                enumAttribute?.Enums,
                parameter.IsOptional
            );
        }

        var function = functionBuilder.Build();
        InitializePropertiesIfNull(function);
        Functions.Add(new FunctionInfo(function, method));
    }

    private static void InitializePropertiesIfNull(FunctionDefinition function)
    {
        function.Parameters ??= new FunctionParameters();
        function.Parameters.Properties ??= new Dictionary<string, FunctionParameterPropertyValue>();
    }

    public void UnloadFunctions()
    {
        Functions = new List<FunctionInfo>();
    }

    public FunctionDefinition? SearchFunction(string functionName)
    {
        return Functions.FirstOrDefault(f => f.Function.Name == functionName)?.Function;
    }

    public async Task<string> ExecuteFunction(FunctionCall call, FunctionContext context)
    {
        var function = Functions.Find(f => f.Function.Name == call.Name);

        if (function is null)
        {
            throw new Exception($"Function {call.Name} does not exist.");
        }

        var moduleInstance = InstantiateModule(function)!;
        moduleInstance.Context = context;

        var parameterValues = ExtractParameterValues(call, function);

        var result = function.Method.Invoke(moduleInstance, parameterValues.ToArray());
        return GetResultString(result);
    }

    private static FunctionModuleBase<FunctionContext>? InstantiateModule(FunctionInfo function)
    {
        return function.Method.DeclaringType!
            .GetConstructor(Type.EmptyTypes)!
            .Invoke(Array.Empty<object>()) as FunctionModuleBase<FunctionContext>;
    }

    private static List<object> ExtractParameterValues(FunctionCall call, FunctionInfo function)
    {
        var parameterValues = new List<object>();

        if (function.Function.Parameters?.Properties is null)
        {
            return parameterValues;
        }

        var methodParameters = function.Method.GetParameters();
        var arguments = JsonNode.Parse(call.Arguments!)!;

        parameterValues.AddRange(
            function.Function.Parameters.Properties
                .Select((_, i) =>
                    function.Function.Parameters.Properties.ToList()[i])
                .Select((param, j) =>
                    arguments[param.Key].Deserialize(methodParameters[j].ParameterType))!);

        return parameterValues;
    }

    private static string GetResultString(object? result)
    {
        if (result is null)
        {
            return "";
        }

        if (result.GetType().IsGenericType && result.GetType().GetGenericTypeDefinition() == typeof(Task<>))
        {
            result = result.GetType().GetProperty("Result")?.GetValue(result);
        }

        return result?.ToString() ?? "";
    }

    private static string FindType(Type type)
    {
        return type switch
        {
            _ when type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal) => "number",
            _ when type == typeof(string) => "string",
            _ when type == typeof(bool) => "boolean",
            _ when type == typeof(IEnumerable) => "array",
            null => "null",
            _ => "object"
        };
    }
}
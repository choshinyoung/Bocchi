using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bocchi.Functions.Attributes;
using OpenAI.ObjectModels.RequestModels;

namespace Bocchi.Functions;

public class FunctionManager
{
    private static readonly TypeInfo ModuleTypeInfo = typeof(FunctionContext).GetTypeInfo();

    public List<FunctionInfo> Functions;

    public FunctionManager()
    {
        Functions = new List<FunctionInfo>();
    }

    public void LoadFunctions()
    {
        var assembly = Assembly.GetEntryAssembly()!;

        var types = assembly.DefinedTypes
            .Where(type => type.IsPublic || type.IsNestedPublic)
            .Where(
                type => ModuleTypeInfo.IsAssignableFrom(type) &&
                        type is { IsAbstract: false, ContainsGenericParameters: false }
            )
            .ToList();

        foreach (var method in types.SelectMany(type => type.GetMethods()))
        {
            var functionAttribute =
                method.GetCustomAttributes(typeof(FunctionAttribute)).FirstOrDefault() as FunctionAttribute;

            if (!method.IsPublic || functionAttribute is null)
            {
                continue;
            }

            var functionBuilder = new FunctionDefinitionBuilder(functionAttribute.Name, functionAttribute.Description);

            foreach (var parameter in method.GetParameters())
            {
                var enumAttribute =
                    parameter.GetCustomAttributes(typeof(EnumValueAttribute)).FirstOrDefault() as
                        EnumValueAttribute;
                var paramAttribute =
                    parameter.GetCustomAttributes(typeof(ParamAttribute)).FirstOrDefault() as
                        ParamAttribute;

                functionBuilder.AddParameter(
                    parameter.Name!,
                    (paramAttribute?.Type ?? parameter.ParameterType.Name).ToLower(),
                    paramAttribute?.Description,
                    enumAttribute?.Enums,
                    parameter.IsOptional
                );
            }

            var function = functionBuilder.Build();
            function.Parameters ??= new FunctionParameters();
            function.Parameters.Properties ??= new Dictionary<string, FunctionParameterPropertyValue>();

            Functions.Add(new FunctionInfo(function, method));
        }
    }

    public void UnloadFunctions()
    {
        Functions = new List<FunctionInfo>();
    }

    public FunctionDefinition? SearchFunction(string function)
    {
        if (Functions.Find(f => f.Function.Name == function) is not null and var result)
        {
            return result.Function;
        }

        return null;
    }

    public async Task<string> ExecuteFunction(FunctionCall call)
    {
        if (Functions.Find(f => f.Function.Name == call.Name) is not (not null and var function))
        {
            throw new Exception($"Function {call.Name} not exists.");
        }

        var moduleBase =
            function.Method.DeclaringType!.GetConstructor(Array.Empty<Type>())!.Invoke(Array.Empty<object>());

        var methodParameters = function.Method.GetParameters();
        var argumentNode = JsonNode.Parse(call.Arguments!)!;
        var parameterValues = new List<object>();

        if (function.Function.Parameters!.Properties is not null)
        {
            parameterValues.AddRange(
                function.Function.Parameters.Properties
                    .Select((_, i) =>
                        function.Function.Parameters.Properties.ToList()[i])
                    .Select((param, j) =>
                        argumentNode[param.Key]!.Deserialize(methodParameters[j].ParameterType)!));
        }

        var result = function.Method.Invoke(moduleBase, parameterValues.ToArray());

        if (result is Task<object> resultTask)
        {
            await resultTask.ConfigureAwait(false);

            return resultTask.Result.ToString() ?? "";
        }

        return result?.ToString() ?? "";
    }
}
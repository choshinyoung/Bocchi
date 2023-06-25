using System.Reflection;
using OpenAI.ObjectModels.RequestModels;

namespace Bocchi.Functions;

public record FunctionInfo(FunctionDefinition Function, MethodInfo Method);
using System.Reflection;
using Bocchi.Extensions.OpenAi.Models.Request;
using Bocchi.Extensions.OpenAi.Models.Response;
using OpenAI.Extensions;
using OpenAI.Managers;

namespace Bocchi.Extensions.OpenAi;

public static class OpenAiServiceExtensions
{
    public static async Task<ChatCompletionWithFunctionCreateResponse> CreateCompletionWithFunction(
        this OpenAIService openAi,
        ChatCompletionWithFunctionCreateRequest chatCompletionCreateRequest, string? modelId = null,
        CancellationToken cancellationToken = default)
    {
        var httpClient = (HttpClient)typeof(OpenAIService).GetField("_httpClient",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(openAi)!;

        chatCompletionCreateRequest.ProcessModelId(modelId, modelId);

        return await httpClient.PostAndReadAsAsync<ChatCompletionWithFunctionCreateResponse>(
            ChatCompletionCreate(), chatCompletionCreateRequest, cancellationToken);
    }

    private static string ChatCompletionCreate()
    {
        return "/v1/chat/completions";
    }
}
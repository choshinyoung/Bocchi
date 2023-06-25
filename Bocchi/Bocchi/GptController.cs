using Bocchi.Utility;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace Bocchi;

public class GptController
{
    private static readonly string AssistantMessage = Setting.Get("ASSISTANT_MESSAGE");

    private static readonly string ConvertSayingMessage = Setting.Get("CONVERT_SAYING_MESSAGE");

    private static readonly string Saying = Setting.Get("SAYING");

    public static readonly List<string> Prefixes = Setting.GetList<string>("PREFIXES");

    public static async Task<string> TalkAsync(string content, List<History>? histories = null, string? apiKey = null)
    {
        return await RequestAsync(AssistantMessage + Saying, content, histories, apiKey);
    }

    public static async Task<string> ConvertAsync(string content, string? apiKey = null)
    {
        return await RequestAsync(ConvertSayingMessage + Saying, content, apiKey: apiKey);
    }

    private static async Task<string> RequestAsync(string systemMessage, string content,
        List<History>? histories = null, string? apiKey = null)
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem(systemMessage)
        };

        if (histories != null)
        {
            messages.AddRange(histories.Select(h =>
                new ChatMessage(
                    h.IsAssistant ? StaticValues.ChatMessageRoles.Assistant : StaticValues.ChatMessageRoles.User,
                    h.Content)));
        }

        messages.Add(ChatMessage.FromUser(content));

        return await RequestMessagesAsync(messages, apiKey);
    }

    private static async Task<string> RequestMessagesAsync(List<ChatMessage> messages, string? apiKey)
    {
        using var openAi = new OpenAIService(new OpenAiOptions { ApiKey = apiKey ?? Config.Get("OPENAI_API_KEY") });

        for (var stackCount = 0; stackCount < 10; stackCount++)
        {
            var functions = BocchiManager.FunctionManager.Functions.Select(f => f.Function).ToList();

            var response = await openAi.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = "gpt-4-0613",
                Functions = functions
            });

            if (!response.Successful)
            {
                if (response.Error == null)
                {
                    throw new Exception("Unknown Error");
                }

                throw new Exception($"{response.Error.Code}: {response.Error.Message}");
            }

            var result = response.Choices.First();

            if (result.FinishReason == "function_call")
            {
                var function = BocchiManager.FunctionManager.SearchFunction(result.Message.FunctionCall!.Name!);

                if (function is null)
                {
                    throw new Exception($"Function {result.Message.FunctionCall!.Name} not exists.");
                }

                messages.Add(ChatMessage.FromFunction(
                    await BocchiManager.FunctionManager.ExecuteFunction(result.Message.FunctionCall!), function.Name));

                continue;
            }

            return result.Message.Content;
        }

        throw new Exception("Too many function recall occured.");
    }
}
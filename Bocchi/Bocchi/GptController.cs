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

        using var openAi = new OpenAIService(new OpenAiOptions
        {
            ApiKey = apiKey ?? Config.Get("OPENAI_API_KEY")
        });

        var result = await openAi.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = Models.Gpt_4
        });

        if (!result.Successful)
        {
            if (result.Error == null)
            {
                throw new Exception("Unknown Error");
            }

            throw new Exception($"{result.Error.Code}: {result.Error.Message}");
        }

        return result.Choices.First().Message.Content;
    }
}
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

    public static readonly OpenAIService OpenAi = new(new OpenAiOptions
    {
        ApiKey = Config.Get("OPENAI_API_KEY")
    });

    public static async Task<string> TalkAsync(string content, List<History>? histories = null)
    {
        return await RequestAsync(AssistantMessage + Saying, content, histories);
    }

    public static async Task<string> ConvertAsync(string content)
    {
        return await RequestAsync(ConvertSayingMessage + Saying, content);
    }

    private static async Task<string> RequestAsync(string systemMessage, string content,
        List<History>? histories = null)
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

        var result = await OpenAi.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
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
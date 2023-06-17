using Bocchi.Extensions.OpenAi;
using Bocchi.Extensions.OpenAi.Models.Request;
using Bocchi.Extensions.OpenAi.Models.Response;
using Bocchi.Utility;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;

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
        using var openAi = new OpenAIService(new OpenAiOptions
        {
            ApiKey = apiKey ?? Config.Get("OPENAI_API_KEY")
        });

        var response = await openAi.CreateCompletionWithFunction(new ChatCompletionWithFunctionCreateRequest
        {
            Messages = messages,
            Model = "gpt-4-0613",
            Functions = new List<Function>
            {
                new()
                {
                    Name = "GetDateTime",
                    Description = "Get current date and time, e.g. '2023년 6월 17일 토요일 오후 5:27:36'",
                    Parameters = new Parameters
                    {
                        ValueType = "object",
                        Properties = new Dictionary<string, Property>()
                    }
                }
            }
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
            if (result.Message.FunctionCall!.Name == "GetDateTime")
            {
                messages.Add(ChatMessage.FromFunction(DateTime.Now.ToString("F"), "GetDateTime"));

                return await RequestMessagesAsync(messages, apiKey);
            }
        }

        return result.Message.Content!;
    }
}
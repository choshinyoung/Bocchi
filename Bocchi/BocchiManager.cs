using Bocchi.Utility;
using OpenAI_API.Models;

namespace Bocchi;

public static class BocchiManager
{
    private static readonly string AssistantMessage = Setting.Get("ASSISTANT_MESSAGE");

    private static readonly string ConvertSayingMessage = Setting.Get("CONVERT_SAYING_MESSAGE");

    private static readonly string Saying = Setting.Get("SAYING");

    public static readonly List<string> Prefixes = Setting.GetList<string>("PREFIXES");

    public static async Task<string> Talk(string content)
    {
        return await Request(AssistantMessage + Saying, content);
    }

    public static async Task<string> Convert(string content)
    {
        return await Request(ConvertSayingMessage + Saying, content);
    }

    private static async Task<string> Request(string systemMessage, string content)
    {
        var chat = OpenAi.Api.Chat.CreateConversation();
        chat.Model = Model.GPT4;

        chat.AppendSystemMessage(systemMessage);

        chat.AppendUserInput(content);
        var response = await chat.GetResponseFromChatbotAsync();

        return response;
    }
}
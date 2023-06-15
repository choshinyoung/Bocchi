using Bocchi.Commands;
using Bocchi.Utility;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace Bocchi;

public static class BocchiManager
{
    private static readonly string AssistantMessage = Setting.Get("ASSISTANT_MESSAGE");

    private static readonly string ConvertSayingMessage = Setting.Get("CONVERT_SAYING_MESSAGE");

    private static readonly string Saying = Setting.Get("SAYING");

    public static readonly List<string> Prefixes = Setting.GetList<string>("PREFIXES");

    public static readonly OpenAIAPI OpenAi = new(new APIAuthentication(Config.Get("OPENAI_API_KEY")));

    public static async Task<string> Talk(string content, List<History>? histories = null)
    {
        return await Request(AssistantMessage + Saying, content, histories);
    }

    public static async Task<string> Convert(string content)
    {
        return await Request(ConvertSayingMessage + Saying, content);
    }

    private static async Task<string> Request(string systemMessage, string content, List<History>? histories = null)
    {
        var chat = OpenAi.Chat.CreateConversation();
        chat.Model = Model.GPT4;

        chat.AppendSystemMessage(systemMessage);

        if (histories != null)
        {
            foreach (var history in histories)
            {
                chat.AppendMessage(history.IsAssistant ? ChatMessageRole.Assistant : ChatMessageRole.User,
                    history.Content);
            }
        }

        chat.AppendUserInput(content);
        var response = await chat.GetResponseFromChatbotAsync();

        return response;
    }
}
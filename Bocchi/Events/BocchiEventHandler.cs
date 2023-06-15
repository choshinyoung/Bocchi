using Discord.WebSocket;

namespace Bocchi.Events;

public class BocchiEventHandler : IEventHandler
{
    public static void Register()
    {
        Bot.Client.MessageReceived += OnMessageReceived;
    }

    private static async Task OnMessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || userMessage.Content == null ||
            userMessage.Author.Id == Bot.Client.CurrentUser.Id || userMessage.Author.IsBot)
        {
            return;
        }

        await BocchiManager.EvaluateMessage(userMessage);
    }
}
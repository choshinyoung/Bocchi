using Discord;

namespace Bocchi.Events;

public class EventHandler : IEventHandler
{
    public static void Register()
    {
        Bot.Client.Log += OnLog;
    }

    private static async Task OnLog(LogMessage message)
    {
        Console.WriteLine(message);

        await Task.CompletedTask;
    }
}
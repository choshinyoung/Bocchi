using Discord.Commands;
using Bocchi.Commands;
using Bocchi.Extensions;
using Discord;
using Discord.WebSocket;

namespace Bocchi.Events;

public class CommandEventHandler : IEventHandler
{
    public static void Register()
    {
        CommandManager.Service.Log += OnLog;

        Bot.Client.MessageReceived += OnMessageReceived;
        CommandManager.Service.CommandExecuted += OnCommandExecuted;
    }

    private static async Task OnLog(LogMessage message)
    {
        Console.WriteLine(message);

        await Task.CompletedTask;
    }

    private static async Task OnMessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage userMessage || userMessage.Content == null ||
            userMessage.Author.Id == Bot.Client.CurrentUser.Id || userMessage.Author.IsBot)
        {
            return;
        }

        await CommandManager.ExecuteCommand(userMessage);
    }

    private static async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context,
        IResult result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        var socketContext = (context as SocketCommandContext)!;

        if (Bot.IsDebugMode)
        {
            await socketContext.ReplyAsync($"Error Occured!\n```{result.ErrorReason}```");
        }
        else
        {
            await socketContext.AddReactionAsync("⚠️");
        }
    }
}
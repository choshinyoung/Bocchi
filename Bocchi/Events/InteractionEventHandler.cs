using Bocchi.Extensions;
using Bocchi.Interactions;
using Bocchi.Utility;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Bocchi.Events;

public class InteractionEventHandler : IEventHandler
{
    public static void Register()
    {
        InteractionManager.Service.Log += OnLog;

        Bot.Client.Ready += OnReady;

        Bot.Client.InteractionCreated += OnInteractionCreated;
        InteractionManager.Service.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    private static async Task OnLog(LogMessage message)
    {
        Console.WriteLine(message);

        await Task.CompletedTask;
    }

    private static async Task OnReady()
    {
        await InteractionManager.Service.RegisterCommandsGloballyAsync();
    }

    private static async Task OnInteractionCreated(SocketInteraction interaction)
    {
        var scope = Bot.Service.CreateScope();
        SocketInteractionContext ctx = new(Bot.Client, interaction);

        await InteractionManager.Service.ExecuteCommandAsync(ctx, scope.ServiceProvider);
    }

    private static async Task OnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context,
        IResult result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        var socketContext = (context as SocketInteractionContext)!;

        if (Bot.IsDebugMode)
        {
            await socketContext.RespondOrFollowupAsync($"{StaticMessages.ErrorMessage}\n```{result.ErrorReason}```",
                true);
        }
        else
        {
            await socketContext.RespondOrFollowupAsync(StaticMessages.ErrorMessage, true);
        }
    }
}
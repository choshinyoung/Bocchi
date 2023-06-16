using Bocchi.Extensions;
using Bocchi.Interactions;
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

        if (result is { IsSuccess: false, Error: InteractionCommandError.UnmetPrecondition })
        {
            await socketContext.RespondAsync("아... 이 커..커맨드를 실행할 권한이 없다고... 생각해요...", true);
        }

        if (Bot.IsDebugMode)
        {
            await socketContext.RespondOrFollowupAsync($"아... 그... 오류가 발생한 것... 같아요...\n```{result.ErrorReason}```",
                true);
        }
        else
        {
            await socketContext.RespondOrFollowupAsync("아... 그... 오류가 발생한 것... 같아요...", true);
        }
    }
}
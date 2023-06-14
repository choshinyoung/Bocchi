using Discord.Interactions;
using Bocchi.Extensions;

namespace Bocchi.Interactions;

public class PingPong : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Just a Ping-Pong Command")]
    public async Task Ping()
    {
        await Context.RespondAsync("Pong!");
    }
}
using Discord.Commands;
using Bocchi.Attributes;
using Bocchi.Extensions;

namespace Bocchi.Commands;

[Order(0)]
public class PingPong : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary("Just a Ping-Pong Command")]
    public async Task Ping()
    {
        await Context.ReplyAsync("Pong!");
    }
}
using Bocchi.Extensions;
using Discord.Commands;

namespace Bocchi.Commands;

public class Bocchi : ModuleBase<SocketCommandContext>
{
    [Command("봇치야")]
    [Alias("봇치짱", "봇치님")]
    [Summary("그게 무슨 소리니")]
    public async Task Call([Remainder] string content)
    {
        await Context.Channel.TriggerTypingAsync();

        await Context.ReplyAsync(await BocchiManager.Talk(content));
    }
}
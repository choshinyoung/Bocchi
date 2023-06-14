using Bocchi.Extensions;
using Discord.Interactions;

namespace Bocchi.Interactions;

public class Bocchi : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("봇치야", "그게 무슨 소리니")]
    public async Task Call(string content)
    {
        await DeferAsync();
        await Context.Channel.TriggerTypingAsync();

        await Context.FollowupAsync(await BocchiManager.Talk(content));
    }
}
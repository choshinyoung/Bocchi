using Bocchi.Extensions;

namespace Bocchi.Commands;

public partial class Bocchi
{
    public async Task Call(string content)
    {
        await Context.Channel.TriggerTypingAsync();

        await Context.ReplyAsync(await BocchiManager.Talk(content));
    }
}
using Bocchi.Extensions;
using Discord;

namespace Bocchi;

public partial class Command
{
    public async Task Call(string content)
    {
        await Context.Channel.TriggerTypingAsync();

        var history = await GetHistoriesAsync(Context.Channel, Context.Message.Reference);

        if (await TryTalkAsync(content, history) is (true, var output))
        {
            await Context.ReplyAsync(output!);
        }
    }

    private static async Task<List<History>> GetHistoriesAsync(IMessageChannel channel,
        MessageReference? reference)
    {
        if (reference == null)
        {
            return new List<History>();
        }

        var message = await channel.GetMessageAsync(reference.MessageId.Value);

        return (await GetHistoriesAsync(channel, message.Reference))
            .Append(new History(message.Content, message.Author.Id == Bot.Client.CurrentUser.Id))
            .ToList();
    }
}
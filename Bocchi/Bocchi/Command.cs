using Bocchi.Extensions;
using Discord;

namespace Bocchi;

public partial class Command
{
    public async Task Call(string? content = null)
    {
        content = content ?? Context.Message.Content;

        await Context.Channel.TriggerTypingAsync();

        await Context.ReplyAsync(await GptController.Talk(content,
            await GetHistoriesAsync(Context.Channel, Context.Message.Reference)));
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
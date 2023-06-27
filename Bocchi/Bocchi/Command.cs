using Bocchi.Extensions;
using Bocchi.Utility;
using Discord;

namespace Bocchi;

public partial class Command
{
    public async Task Call(string content)
    {
        await Context.Channel.TriggerTypingAsync();

        var history = await GetHistoriesAsync(Context.Channel, Context.Message.Reference);

        if (await TryTalkAsync(content, history) is not (true, var result))
        {
            await Context.ReplyAsync(StaticMessages.TrialEndMessage);

            return;
        }

        if (BocchiManager.CheckTrialCount(Context.User.Id) is not (true, var isAvailable))
        {
            await Context.ReplyAsync(result);

            return;
        }

        if (isAvailable)
        {
            await Context.ReplyAsync(
                $"{result}\n\n{string.Format(StaticMessages.TrialNoticeMessage, BocchiManager.TrialCount, BocchiManager.GetTrialCount(Context.User.Id))}");
        }
        else
        {
            await Context.ReplyAsync($"{result}\n\n{StaticMessages.TrialEndMessage}");
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
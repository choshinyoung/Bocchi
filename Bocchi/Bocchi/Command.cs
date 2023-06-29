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

        var result = await TryTalkAsync(content, history);

        if (!result.isSuccess)
        {
            await Context.ReplyAsync(StaticMessages.TrialEndMessage);

            return;
        }

        if (result.output is null)
        {
            return;
        }

        if (BocchiManager.CheckTrialCount(Context.User.Id) is not (true, var isAvailable))
        {
            await Context.ReplyAsync(result.output);

            return;
        }

        if (isAvailable)
        {
            await Context.ReplyAsync(
                $"{result.output}\n\n{string.Format(StaticMessages.TrialNoticeMessage, BocchiManager.TrialCount, BocchiManager.GetTrialCount(Context.User.Id))}");
        }
        else
        {
            await Context.ReplyAsync($"{result.output}\n\n{StaticMessages.TrialEndMessage}");
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
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

        if (BocchiManager.CheckTrialCount(Context.User.Id) is (true, var isAvailable))
        {
            if (isAvailable)
            {
                await Context.ReplyAsync(
                    $"아... 무료 요청 {BocchiManager.TrialCount}개 중에, 그... {BocchiManager.GetTrialCount(Context.User.Id)}개를 사용한 것 같아요...");
            }
            else
            {
                await Context.ReplyAsync(
                    "무료 요청을 다 사용하신 거 같아요... 그... `/등록`이라는... 슬래시 커맨드를... 사용해서... OpenAI API 키를 등록하시면... 될 것 같아요...\n키는 다음 페이지에서 생성할 수 있다고... 생각해요...\nhttps://platform.openai.com/account/api-keys");
            }
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
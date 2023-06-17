using Bocchi.Extensions;
using Bocchi.Utility.Database;
using Discord;
using Discord.Commands;
using Fergun.Interactive;

namespace Bocchi;

public partial class Command
{
    public Command(SocketCommandContext context, InteractiveService interactive)
    {
        Context = context;
        Interactive = interactive;
    }

    private InteractiveService Interactive { get; }

    private SocketCommandContext Context { get; }

    private async Task<(bool isSuccess, string? output)> TryTalkAsync(string content,
        List<History>? histories = null)
    {
        if (BocchiManager.CheckTrialCount(Context.User.Id) is { isTrial: true, isAvailable: false })
        {
            return (false, null);
        }

        try
        {
            BocchiManager.UpdateTrialCount(Context.User.Id);

            var user = DbManager.GetUser(Context.User.Id);

            return (true, await GptController.TalkAsync(content, histories, user.IsTrial ? null : user.OpenAiKey));
        }
        catch (Exception ex)
        {
            var builder = new ComponentBuilder()
                .WithButton("다시 시도하기", "retry");

            Console.WriteLine(ex);

            var message = await Context.ReplyAsync($"아... 그... 요청 처리 중에 오류가 발생한 것 같아요...\n```{ex.Message}```",
                component: builder.Build());

            var result = await Interactive.NextMessageComponentAsync(x => x.Message.Id == message.Id,
                timeout: TimeSpan.FromMinutes(1));

            if (result.IsSuccess)
            {
                await result.Value.DeferAsync();

                await TryTalkAsync(content, histories);
            }
        }

        return (false, null);
    }
}
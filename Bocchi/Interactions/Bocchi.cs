using Bocchi.Extensions;
using Bocchi.Functions;
using Bocchi.Utility.Database;
using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Bocchi.Interactions;

public class Bocchi : InteractionModuleBase<SocketInteractionContext>
{
    public InteractiveService? Interactive { get; set; }

    [SlashCommand("봇치야", "그게 무슨 소리니")]
    public async Task Call(string content)
    {
        await DeferAsync();

        if (await TryTalkAsync(content) is (true, var result))
        {
            if (BocchiManager.CheckTrialCount(Context.User.Id) is (true, var isAvailable))
            {
                if (isAvailable)
                {
                    await Context.FollowupAsync(result! +
                                                $"\n\n아... 무료 요청 {BocchiManager.TrialCount}개 중에, 그... {BocchiManager.GetTrialCount(Context.User.Id)}개를 사용한 것 같아요...");
                }
                else
                {
                    await Context.FollowupAsync(result! +
                                                "\n\n무료 요청을 다 사용하신 거 같아요... 그... `/등록`이라는... 슬래시 커맨드를... 사용해서... OpenAI API 키를 등록하시면... 될 것 같아요...\n키는 다음 페이지에서 생성할 수 있다고... 생각해요...\nhttps://platform.openai.com/account/api-keys");
                }
            }
            else
            {
                await Context.FollowupAsync(result!);
            }
        }
    }

    private async Task<(bool isSuccess, string? output)> TryTalkAsync(
        string content, List<History>? histories = null)
    {
        if (BocchiManager.CheckTrialCount(Context.User.Id) is { isTrial: true, isAvailable: false })
        {
            return (false, null);
        }

        try
        {
            BocchiManager.UpdateTrialCount(Context.User.Id);

            var user = DbManager.GetUser(Context.User.Id);

            var context = new FunctionContext
            {
                History = Array.Empty<History>(),
                User = Context.User,
                Channel = Context.Channel,
                Guild = Context.Guild,
                Interaction = Context.Interaction
            };

            return (true,
                await GptController.TalkAsync(content, context, histories, user.IsTrial ? null : user.OpenAiKey));
        }
        catch (Exception ex)
        {
            var builder = new ComponentBuilder()
                .WithButton("다시 시도하기", "retry");

            var message =
                await Context.FollowupAsync($"아... 그... 요청 처리 중에 오류가 발생한 것 같아요...\n```{ex.Message}```",
                    component: builder.Build());

            var result = await Interactive!.NextMessageComponentAsync(x => x.Message.Id == message.Id,
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
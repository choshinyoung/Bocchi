using Bocchi.Extensions;
using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Bocchi.Interactions;

public class Bocchi : InteractionModuleBase<SocketInteractionContext>
{
    public InteractiveService Interactive { get; set; }

    [SlashCommand("봇치야", "그게 무슨 소리니")]
    public async Task Call(string content)
    {
        await DeferAsync();
        await Context.Channel.TriggerTypingAsync();

        if (await TryTalkAsync(content) is var (isSuccess, output) && isSuccess)
        {
            await Context.FollowupAsync(output!);
        }
    }

    private async Task<(bool isSuccess, string? output)> TryTalkAsync(string content,
        List<History>? histories = null)
    {
        try
        {
            return (true, await GptController.TalkAsync(content, histories));
        }
        catch (Exception ex)
        {
            var builder = new ComponentBuilder()
                .WithButton("다시 시도하기", "retry");

            var message =
                await Context.FollowupAsync($"요청 처리 중 오류 발생!\n```{ex.Message}```", component: builder.Build());

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
using Bocchi.Extensions;
using Bocchi.Functions;
using Bocchi.Utility;
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

        var result = await TryTalkAsync(content);

        if (!result.isSuccess)
        {
            await Context.FollowupAsync(StaticMessages.TrialEndMessage);

            return;
        }

        if (result.output is null)
        {
            return;
        }

        if (BocchiManager.CheckTrialCount(Context.User.Id) is not (true, var isAvailable))
        {
            await Context.FollowupAsync(result.output);

            return;
        }

        if (isAvailable)
        {
            await Context.FollowupAsync(
                $"{result.output}\n\n{string.Format(StaticMessages.TrialNoticeMessage, BocchiManager.TrialCount, BocchiManager.GetTrialCount(Context.User.Id))}");
        }
        else
        {
            await Context.FollowupAsync($"{result.output}\n\n{StaticMessages.TrialEndMessage}");
        }
    }

    private async Task<(bool isSuccess, string? output)> TryTalkAsync(
        string content, List<History>? histories = null)
    {
        if (BocchiManager.CheckTrialCount(Context.User.Id) is { isTrial: true, isAvailable: false })
        {
            return (false, null);
        }

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

        try
        {
            var response =
                await GptController.TalkAsync(content, context, histories, user.IsTrial ? null : user.OpenAiKey);
            return (true, response);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(content, histories, ex);
        }
    }

    private async Task<(bool isSuccess, string? output)> HandleExceptionAsync(string content, List<History>? histories,
        Exception ex)
    {
        var builder = new ComponentBuilder()
            .WithButton("다시 시도하기", "retry");

        var message = await Context.FollowupAsync(
            $"{StaticMessages.ErrorMessage}\n```{ex.Message}```",
            component: builder.Build()
        );

        var result = await Interactive!.NextMessageComponentAsync(
            x => x.Message.Id == message.Id,
            timeout: TimeSpan.FromMinutes(1)
        );

        if (!result.IsSuccess)
        {
            return (true, null);
        }

        await result.Value.DeferAsync();
        return await TryTalkAsync(content, histories);
    }
}
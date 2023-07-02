using Bocchi.Extensions;
using Bocchi.Functions;
using Bocchi.Utility;
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

    private async Task<(bool isSuccess, string? output)> TryTalkAsync(string content, List<History>? histories = null)
    {
        if (BocchiManager.CheckTrialCount(Context.User.Id) is { isTrial: true, isAvailable: false })
        {
            return (false, null);
        }

        BocchiManager.UpdateTrialCount(Context.User.Id);
        var user = DbManager.GetUser(Context.User.Id);

        var context = new FunctionContext
        {
            History = histories?.ToArray() ?? Array.Empty<History>(),
            User = Context.User,
            Channel = Context.Channel,
            Guild = Context.Guild,
            Message = Context.Message,
            ApiKey = user.IsTrial ? null : user.OpenAiKey
        };

        try
        {
            var response =
                await GptController.TalkAsync(content, context, histories, user.IsTrial ? null : user.OpenAiKey);
            return (true, response);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, content, histories);
        }
    }

    private async Task<(bool isSuccess, string? output)> HandleExceptionAsync(Exception ex, string content,
        List<History>? histories)
    {
        var builder = new ComponentBuilder()
            .WithButton("다시 시도하기", "retry");

        var message = await Context.ReplyAsync(
            $"{StaticMessages.ErrorMessage}\n```{ex.Message}```",
            component: builder.Build()
        );

        var buttonResponse = await Interactive.NextMessageComponentAsync(
            x => x.Message.Id == message.Id,
            timeout: TimeSpan.FromMinutes(1)
        );

        if (!buttonResponse.IsSuccess)
        {
            return (true, null);
        }

        await buttonResponse.Value.DeferAsync();
        return await TryTalkAsync(content, histories);
    }
}
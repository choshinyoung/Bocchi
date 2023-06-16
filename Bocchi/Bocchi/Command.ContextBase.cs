﻿using Bocchi.Extensions;
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
        try
        {
            return (true, await GptController.TalkAsync(content, histories));
        }
        catch (Exception ex)
        {
            var builder = new ComponentBuilder()
                .WithButton("다시 시도하기", "retry");

            var message = await Context.ReplyAsync($"요청 처리 중 오류 발생!\n```{ex.Message}```", component: builder.Build());

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
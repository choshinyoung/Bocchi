using Bocchi.Events;
using Discord.Commands;
using Discord.WebSocket;

namespace Bocchi;

public static class BocchiManager
{
    public static async Task Initialize()
    {
        BocchiEventHandler.Register();

        await Task.CompletedTask;
    }

    public static async Task EvaluateMessage(SocketUserMessage message)
    {
        SocketCommandContext context = new(Bot.Client, message);

        var argPos = 0;
        if (GptController.Prefixes.Any(prefix => message.HasStringPrefix(prefix, ref argPos)))
        {
            await new Command(context).Call();

            return;
        }

        if (message.HasMentionPrefix(Bot.Client.CurrentUser, ref argPos) ||
            message.HasStringPrefix($"<@{Bot.Client.CurrentUser.Id}>", ref argPos))
        {
            await new Command(context).Call(context.Message.Content[argPos..]);

            return;
        }

        if (message.Reference != null)
        {
            var reference = await context.Channel.GetMessageAsync(message.Reference.MessageId.Value);

            if (reference.Author.Id == Bot.Client.CurrentUser.Id)
            {
                await new Command(context).Call();
            }
        }
    }
}
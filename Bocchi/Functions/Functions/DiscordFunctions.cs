using Bocchi.Extensions;
using Bocchi.Functions.Attributes;
using Bocchi.Utility;
using Discord;

namespace Bocchi.Functions.Functions;

public class DiscordFunctions : FunctionModuleBase<FunctionContext>
{
    [Function("GetUserInfo",
        "Returns the information of current Discord user (including data of username, nickname, isBot, id, discriminator, status, createdAt, activities)")]
    public Task<string> GetUserInfo()
    {
        var user = Context.User;

        return Task.FromResult($$"""
                                 {
                                     username: {{user.Username}},
                                     nickname: {{user.GetName()}},
                                     Discriminator: {{user.Discriminator}},
                                     id: {{user.Id}},
                                     isBot: {{user.IsBot}},
                                     createdAt: {{user.CreatedAt.ToKstTime():F}},
                                     avatarUrl: {{user.GetAvatarUrl()}},
                                     status: {{user.Status}},
                                     activities: [{{string.Join(", ", user.Activities.Select(a => $"{a.Type} - {a.Name}: {a.Details}"))}}]
                                 }
                                 """);
    }

    [Function("GetRecentChatting",
        """
        Returns recent chat messages for the current discord channel.
        Use this when users mention about recent chat or recent conversation or when they need information from previous chats.
        Messages are given in order from most recently sent to least recently sent.
        Examples of user message: '대화 요약해줘', '이게 무슨 뜻이야', '저분이 뭐라고 말하는거야', '저 사람이 왜 저런 말을 하는거야?', '저분이 말하는게 뭐야?'...
        """
    )]
    public async Task<string> GetRecentChatting(
        [Param(
            "The 'page' parameter fetches sets of 30 recent discord messages. 1 for the latest, 2 for the next, etc."
        )]
        int page)
    {
        var rawMessages = Context.Channel.GetMessagesAsync(30 * page).GetAsyncEnumerator();

        var messages = new List<IMessage>();

        while (await rawMessages.MoveNextAsync())
        {
            messages.AddRange(rawMessages.Current);
        }

        return string.Join("\n",
            messages
                .TakeLast(30)
                .Reverse()
                .Where(m => m.Id != Context.Message?.Id)
                .Select(msg => $"{msg.Author.Username}: {msg.Content}")
        );
    }

    [Function("React", "Add reaction to user message. the function's return value is a success or not.")]
    public async Task<bool> React([Param("emote should be unicode emoji character")] string emote)
    {
        if (Context.Message is null)
        {
            return false;
        }

        await Context.Message.AddReactionAsync(new Emoji(emote));

        return true;
    }
}
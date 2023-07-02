using Bocchi.Extensions;
using Bocchi.Functions.Attributes;
using Bocchi.Utility;
using Discord;
using Discord.WebSocket;

namespace Bocchi.Functions.Functions;

public class DiscordFunctions : FunctionModuleBase<FunctionContext>
{
    [Function("GetCurrentUserInfo",
        "Returns the information of current Discord user (including data of username, nickname, discriminator, id, isBot, createdAt, avatarUrl, status, activities)")]
    public Task<string> GetCurrentUserInfo()
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

    [Function("GetUserInfo",
        "Returns the Discord user information with given user indicator (including data of username, nickname, discriminator, id, isBot, createdAt, avatarUrl, status, activities)")]
    public async Task<string> GetUserInfo(
        [Param("Parameter 'userIndicator' will be user id, username or nickname.")]
        string userIndicator)
    {
        if (Context.Guild is null)
        {
            return "Request failed: Guild not exist";
        }

        SocketUser? user = null;

        if (ulong.TryParse(userIndicator, out var id))
        {
            user = Context.Guild.GetUser(id);
        }
        else if (Context.Guild.Users.First(g =>
                     g.Username == userIndicator ||
                     g.Nickname == userIndicator ||
                     g.DisplayName == userIndicator)
                 is not null and var u)
        {
            user = u;
        }
        else if (await Context.Guild.SearchUsersAsync(userIndicator) is var searchResults)
        {
            if (searchResults.Any())
            {
                user = Context.Guild.GetUser(searchResults.First().Id);
            }
        }

        return user is null
            ? "Request failed: user not exist"
            : $$"""
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
                """;
    }

    [Function("GetCurrentChannelInfo",
        "Returns the information of current Discord channel (including data of name, id, topic, createdAt, category, isNsfw)")]
    public Task<string> GetCurrentChannelInfo()
    {
        var channel = Context.Channel;
        var textChannel = Context.Channel as SocketTextChannel;

        return Task.FromResult($$"""
                                 {
                                     name: {{channel.Name}},
                                     id: {{channel.Id}},
                                     topic : {{textChannel?.Topic}},
                                     createdAt: {{channel.CreatedAt.ToKstTime():F}},
                                     category: {{textChannel?.Category}},
                                     isNsfw: {{textChannel?.IsNsfw}}
                                 }
                                 """);
    }

    [Function("GetChannelInfo",
        "Returns the Discord channel information with given user indicator (including data of name, chanelType, id, topic, createdAt, category, isNsfw)")]
    public Task<string> GetChannelInfo(
        [Param("Parameter 'channelIndicator' will be channel id or name")]
        string channelIndicator)
    {
        if (Context.Guild is null)
        {
            return Task.FromResult("Request failed: Guild not exist");
        }

        SocketChannel? channel = null;

        if (ulong.TryParse(channelIndicator, out var id))
        {
            channel = Context.Guild.GetChannel(id);
        }
        else if (Context.Guild.Channels.First(g => g.Name == channelIndicator) is not null and var c)
        {
            channel = c;
        }

        if (channel is null)
        {
            return Task.FromResult("Request failed: channel not exist");
        }

        var messageChannel = channel as IMessageChannel;
        var textChannel = channel as SocketTextChannel;

        var channelType = channel switch
        {
            IVoiceChannel => "VoiceChannel",
            IThreadChannel => "ThreadChannel",
            ITextChannel => "TextChannel",
            IForumChannel => "ForumChannel",
            _ => "OtherChannel"
        };

        return Task.FromResult($$"""
                                 {
                                     name: {{messageChannel?.Name}},
                                     id: {{channel.Id}},
                                     channelType: {{channelType}},
                                     topic : {{textChannel?.Topic}},
                                     createdAt: {{channel.CreatedAt.ToKstTime():F}},
                                     category: {{textChannel?.Category}},
                                     isNsfw: {{textChannel?.IsNsfw}}
                                 }
                                 """);
    }

    [Function("GetCurrentServerInfo",
        "Returns the information of current Discord server (or guild) (including data of name, id, description, createdAt, iconUrl, owner, memberCount, boostCount, events)")]
    public async Task<string> GetCurrentServerInfo()
    {
        var guild = Context.Guild;

        return guild is null
            ? "Request failed: Guild not exist"
            : $$"""
                {
                    name: {{guild.Name}},
                    id: {{guild.Id}},
                    description : {{guild.Description}},
                    createdAt: {{guild.CreatedAt.ToKstTime():F}},
                    iconUrl: {{guild.IconUrl}},
                    owner: {{guild.Owner.Username}},
                    memberCount: {{guild.MemberCount}},
                    boostCount: {{guild.PremiumSubscriptionCount}},
                    events: [{{string.Join(", ", (await guild.GetEventsAsync()).Select(e => $"{e.Type} - {e.Name}: {e.Description}, location: {e.Location}, startTime: {e.StartTime.ToKstTime():F}, status: {e.Status}, userCount: {e.UserCount}"))}}]
                }
                """;
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
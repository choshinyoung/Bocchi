using Bocchi.Extensions;
using Bocchi.Functions.Attributes;
using Bocchi.Utility;
using Discord;

namespace Bocchi.Functions.Functions;

public class DiscordFunctions : FunctionModuleBase<FunctionContext>
{
    [Function("GetUserInfo",
        "Returns the information of current Discord user (including data of username, nickname, isBot, id, discriminator, status, createdAt, activities)")]
    public string GetUserInfo()
    {
        var user = Context.User;

        return $$"""
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
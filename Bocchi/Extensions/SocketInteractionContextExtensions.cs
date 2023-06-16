using Discord;
using Discord.Interactions;
using Discord.Rest;

namespace Bocchi.Extensions;

public static class SocketInteractionContextExtensions
{
    public static async Task RespondAsync(this SocketInteractionContext context, object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        await context.Interaction.RespondAsync(content.ToString(), ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }

    public static async Task<RestFollowupMessage> FollowupAsync(this SocketInteractionContext context, object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        return await context.Interaction.FollowupAsync(content.ToString(), ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }

    public static async Task<RestFollowupMessage?> RespondOrFollowupAsync(this SocketInteractionContext context,
        object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        if (context.Interaction.HasResponded)
        {
            return await context.FollowupAsync(content, ephemeral, disableMention, component);
        }

        await context.RespondAsync(content, ephemeral, disableMention, component);

        return null;
    }

    public static async Task<RestUserMessage> SendAsync(this SocketInteractionContext context, object content,
        bool disableMention = true, MessageComponent? component = null, MessageReference? messageReference = null)
    {
        return await context.Channel.SendMessageAsync(content.ToString(),
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component,
            messageReference: messageReference);
    }

    public static async Task RespondEmbedAsync(this SocketInteractionContext context, object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithDescription(content.ToString()).Build();

        await context.Interaction.RespondAsync(embed: embed, ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }

    public static async Task RespondEmbedAsync(this SocketInteractionContext context, Embed embed,
        string? content = null,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        await context.Interaction.RespondAsync(content, embed: embed, ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }
}
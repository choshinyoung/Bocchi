﻿using System.Text.Json.Serialization;
using OpenAI.ObjectModels;

namespace Bocchi.Extensions.OpenAi.Models.Response;

/// <summary>
///     The contents of the message.
///     Messages must be an array of message objects, where each object has a role (either “system”, “user”, or
///     “assistant”) and content (the content of the message) and an optional name
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// </summary>
    /// <param name="role">The role of the author of this message. One of system, user, or assistant.</param>
    /// <param name="content">The contents of the message.</param>
    /// <param name="name">
    ///     The name of the author of this message. May contain a-z, A-Z, 0-9, and underscores, with a maximum
    ///     length of 64 characters.
    /// </param>
    public ChatMessage(string role, string content, string? name = null)
    {
        Role = role;
        Content = content;
        Name = name;
    }

    /// <summary>
    ///     The role of the author of this message. One of system, user, or assistant.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }

    /// <summary>
    ///     The contents of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    ///     The name of the author of this message. May contain a-z, A-Z, 0-9, and underscores, with a maximum length of 64
    ///     characters.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("function_call")] public FunctionCall? FunctionCall { get; set; }

    public static ChatMessage FromAssistant(string content, string? name = null)
    {
        return new ChatMessage(StaticValues.ChatMessageRoles.Assistant, content, name);
    }

    public static ChatMessage FromUser(string content, string? name = null)
    {
        return new ChatMessage(StaticValues.ChatMessageRoles.User, content, name);
    }

    public static ChatMessage FromSystem(string content, string? name = null)
    {
        return new ChatMessage(StaticValues.ChatMessageRoles.System, content, name);
    }

    public static ChatMessage FromFunction(string content, string name = null)
    {
        return new ChatMessage("function", content, name);
    }
}
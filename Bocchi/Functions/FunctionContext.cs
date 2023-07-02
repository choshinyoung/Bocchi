using Discord;
using Discord.WebSocket;

namespace Bocchi.Functions;

public class FunctionContext
{
    public string? ApiKey { init; get; }

    public History[] History { init; get; }

    public SocketUser User { init; get; }
    public IMessageChannel Channel { init; get; }
    public SocketGuild? Guild { init; get; }

    public SocketMessage? Message { init; get; }
    public SocketInteraction? Interaction { init; get; }
}
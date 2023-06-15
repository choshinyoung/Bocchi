using Discord.Commands;

namespace Bocchi;

public partial class Command
{
    public Command(SocketCommandContext context)
    {
        Context = context;
    }

    private SocketCommandContext Context { get; }
}
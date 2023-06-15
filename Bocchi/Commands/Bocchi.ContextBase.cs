using Discord.Commands;

namespace Bocchi.Commands;

public partial class Bocchi
{
    public Bocchi(SocketCommandContext context)
    {
        Context = context;
    }

    private SocketCommandContext Context { get; }
}
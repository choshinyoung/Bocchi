using Bocchi.Commands;
using Bocchi.Interactions;
using Bocchi.Utility;
using Discord;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.Extensions.DependencyInjection;
using EventHandler = Bocchi.Events.EventHandler;

namespace Bocchi;

public static class Bot
{
    public static readonly bool IsDebugMode = Config.GetBool("DEBUG_MODE");

    private static readonly DiscordSocketConfig ClientConfig = new()
    {
        GatewayIntents = GatewayIntents.All,
        LogLevel = IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly DiscordSocketClient Client = new(ClientConfig);

    public static readonly IServiceProvider Service = new ServiceCollection()
        .AddSingleton(Client)
        .AddSingleton<InteractiveService>()
        .AddSingleton(InteractionManager.Service)
        .AddSingleton(CommandManager.Service)
        .BuildServiceProvider();

    public static async Task Start()
    {
        EventHandler.Register();

        await InteractionManager.Initialize();
        await CommandManager.Initialize();

        await Client.LoginAsync(TokenType.Bot, Config.Get("TOKEN"));
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}
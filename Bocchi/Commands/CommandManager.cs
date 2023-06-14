﻿using System.Reflection;
using Bocchi.Events;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bocchi.Commands;

public static class CommandManager
{
    private static readonly CommandServiceConfig CommandConfig = new()
    {
        DefaultRunMode = RunMode.Async,
        LogLevel = Bot.IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly CommandService Service = new(CommandConfig);

    public static bool IsEnabled { get; private set; }

    public static async Task Initialize()
    {
        IsEnabled = true;

        await LoadModulesAsync();

        CommandEventHandler.Register();
    }

    public static async Task LoadModulesAsync()
    {
        await Service.AddModulesAsync(Assembly.GetEntryAssembly(), Bot.Service);
    }

    public static async Task UnloadModulesAsync()
    {
        foreach (var module in Service.Modules)
        {
            await Service.RemoveModuleAsync(module);
        }
    }

    public static async Task ExecuteCommand(SocketUserMessage message)
    {
        SocketCommandContext context = new(Bot.Client, message);

        if (Service.Search(context, 0).IsSuccess)
        {
            await Service.ExecuteAsync(context, 0, Bot.Service);
        }
    }
}
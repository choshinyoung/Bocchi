using System.Reflection.Metadata;
using Bocchi.Commands;
using Bocchi.Events;
using Bocchi.Interactions;

[assembly: MetadataUpdateHandler(typeof(HotReloadHandler))]

namespace Bocchi.Events;

public class HotReloadHandler
{
    public static void UpdateApplication(Type[]? types)
    {
        Task.Run(async () => await ReloadModules());
    }

    private static async Task ReloadModules()
    {
        if (InteractionManager.IsEnabled)
        {
            await InteractionManager.UnloadModulesAsync();
            await InteractionManager.LoadModulesAsync();

            await InteractionManager.Service.RegisterCommandsGloballyAsync();
        }

        if (CommandManager.IsEnabled)
        {
            await CommandManager.UnloadModulesAsync();
            await CommandManager.LoadModulesAsync();
        }

        BocchiManager.FunctionManager.UnloadFunctions();
        BocchiManager.FunctionManager.LoadFunctions();

        Console.WriteLine("Reload complete.");
    }
}
using Discord.Interactions;
using Bocchi.Attributes;
using Bocchi.Utility;
using Discord;

namespace Bocchi.Interactions.AutoCompletes;

public class SlashCommandNameAutoComplete : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        var modules = Help.GetSlashCommandModules();

        var commands = modules
            .SelectMany(Help.GetSlashCommandsFromModule)
            .Where(c => !InfoUtility.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Description))
            .DistinctBy(Help.GetFullCommandName)
            .ToList();

        return Task.FromResult(AutocompletionResult.FromSuccess(commands.Select(c =>
            new AutocompleteResult(Help.GetFullCommandName(c), Help.GetFullCommandName(c)))));
    }
}
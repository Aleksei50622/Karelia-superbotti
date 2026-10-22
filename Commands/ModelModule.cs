using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Superbotti.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbotti.Commands
{
    /// <summary>
    /// Contains bot commands related to LocalAI models
    /// </summary>
    /// <param name="models"></param>
    [SlashCommand("models", "Select the active models")]
    public class ModelModule(Models models): ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("text", "Set model used for text generation")]
        public async Task SetText([SlashCommandParameter(AutocompleteProviderType = typeof(ChatModelAutocomplete)) ] string model)
        {
            models.TextModel = model;
            await RespondAsync(InteractionCallback.Message($"Updated text model to `{model}`"));
        }

        [SubSlashCommand("speech", "Set model used for speech")]
        public async Task SetSpeech([SlashCommandParameter(AutocompleteProviderType = typeof(SpeechModelAutocomplete)) ] string model)
        {
            models.SpeechModel = model;
            await RespondAsync(InteractionCallback.Message($"Updated speech model to `{model}`"));
        }
    }

    public abstract class ModelAutocompleteBase(Func<Task<IEnumerable<string>>> source)
    : IAutocompleteProvider<AutocompleteInteractionContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(
            ApplicationCommandInteractionDataOption option,
            AutocompleteInteractionContext context)
        {
            var modelList = await source();

            return modelList
                .Take(25) // Discord has a limit of 25 choices
                .Select(m => new ApplicationCommandOptionChoiceProperties(m, m));
        }
    }

    /// <summary>
    /// Autocomplete for chat models
    /// </summary>
    /// <param name="models"></param>
    public class ChatModelAutocomplete(Models models)
        : ModelAutocompleteBase(models.GetChatModelsAsync);

    /// <summary>
    /// Autocomplete for speech models
    /// </summary>
    /// <param name="models"></param>
    public class SpeechModelAutocomplete(Models models)
        : ModelAutocompleteBase(models.GetSpeechModelsAsync);
}

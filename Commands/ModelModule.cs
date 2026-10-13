using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Superbotti.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Superbotti.Commands
{
    // /models text ja /models speech antaa valita saatavista modeleista, tällä hetkellä vain listaa
    // Ehkä kannattaa cachettaa
    // Tallentaa ne johonkin asetusluokkaan/tiedostoon?
    [SlashCommand("models", "Select the active models")]
    public class ModelModule(Models models): ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("text", "Set model used for text generation")]
        public async Task SetText()
        {
            var modelList = models.GetChatModelsAsync;
            var asString = string.Join("\n",modelList);
            await RespondAsync(InteractionCallback.Message(asString));
        }
        [SubSlashCommand("speech", "Set model used for speech")]
        public async Task SetSpeech()
        {
            var modelList = models.GetSpeechModelsAsync;
            var asString = string.Join("\n", modelList);
            await RespondAsync(InteractionCallback.Message(asString));
        }

    }
}

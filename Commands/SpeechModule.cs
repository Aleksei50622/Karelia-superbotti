using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Superbotti.Services;

namespace Superbotti.Commands
    {
        public class SpeechModule(SpeechService speech): ApplicationCommandModule<ApplicationCommandContext>
    {
        [SlashCommand("speak", "Provide audio of the previous message")]
        public async Task TextToSpeech()
        {
            await RespondAsync(InteractionCallback.DeferredMessage());

            try
            {
                string text = await GetPreviousMessageAsync();
                Stream audio = await speech.TextToSpeechAsync(text);

                await FollowupAsync(new InteractionMessageProperties()
                    .AddAttachments(new AttachmentProperties("message.wav", audio)));
            }
            catch (Exception)
            {
                await FollowupAsync("Speech failed.");
            }
        }

        private async Task<string?> GetPreviousMessageAsync()
        {
            var messages = Context.Client.Rest.GetMessagesAsync(Context.Channel.Id,
                new PaginationProperties<ulong>
                {
                    Direction = PaginationDirection.Before,
                    BatchSize = 10
                });

            await foreach (var message in messages)
            {
                if (!string.IsNullOrWhiteSpace(message.Content))
                    return message.Content;
            }

            return null;
        }
    }
}

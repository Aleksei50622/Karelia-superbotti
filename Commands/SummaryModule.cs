using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Superbotti.Services;

namespace Superbotti.Commands
{
    /// <summary>
    /// Contains bot commands related to summarizing content
    /// </summary>
    /// <param name="speech"></param>
    /// <param name="summaries"></param>
    public class SummaryModule(SpeechService speech, SummaryService summaries) : ApplicationCommandModule<ApplicationCommandContext>
    {

        [SlashCommand("summary", "Summarize recent chat messages")]
        public async Task AISummary(
        [SlashCommandParameter(Name = "count", Description = "How many messages to summarize (10-100)", MinValue = 10, MaxValue = 100)]
        int messageCount = 50)
        {
            await RespondAsync(InteractionCallback.DeferredMessage());

            try
            {
                string chat = await GetRecentMessagesAsync(messageCount);
                string chatSummary = await summaries.SummarizeAsync(chat);
                Stream audioResponse = await speech.TextToSpeechAsync(chatSummary);

                var message = new MessageProperties().WithContent(chatSummary).AddAttachments(new AttachmentProperties("summary.wav", audioResponse));

                //Käyttäjälle lähtevä viesti
                DMChannel userDM = await Context.User.GetDMChannelAsync();
                await userDM.SendMessageAsync(message);

                // Kanavalle lähtevä viesti
                await FollowupAsync("Sent");
            }
            catch (HttpRequestException)
            {
                await FollowupAsync("Could not connect to LocalAI.\nIs the container running?");
            }
            catch (Exception ex)
            {
                // Voi mahdollisesti printata salaisuuksia
                await FollowupAsync($"Summary failed: {ex.Message}");
            }
        }
        /// <summary>
        /// Fetches messages from Discords REST api from current chat channel up to the given limit
        /// </summary>
        /// <param name="messageLimit"></param>
        /// <returns></returns>
        private async Task<string> GetRecentMessagesAsync(int messageLimit)
        {
            List<string> lines = new();
            var messages = Context.Client.Rest.GetMessagesAsync(Context.Channel.Id,
                new PaginationProperties<ulong>
                {
                    Direction = PaginationDirection.Before,
                    BatchSize = messageLimit
                }
            );

            await foreach (RestMessage message in messages)
            {
                // Hylkää kaikki tyhjät ja bottien tekemät viestit listasta
                if (string.IsNullOrWhiteSpace(message.Content) || message.Author.IsBot)
                    continue;

                // Lisätään käyttäjän nimi jokaisen viestin alkuun AI:n tunnistettavaksi
                lines.Add($"{message.Author.GlobalName}: {message.Content}");

                if (lines.Count >= messageLimit)
                    break;
            }
            lines.Reverse();
            return string.Join("\n", lines);
        }
    }
}
using NetCord;
using NetCord.Gateway;
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
                DMChannel userDM = await Context.User.GetDMChannelAsync();

                var message = new MessageProperties().AddAttachments(new AttachmentProperties("summary.wav", audioResponse));
                if (chatSummary.Length > 1900)
                {
                    message.WithEmbeds([new EmbedProperties().WithDescription(chatSummary)]);
                }
                else
                {
                    message.WithContent(chatSummary);
                }

                //Käyttäjälle lähtevä viesti
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
            var guildUsers = Context.Guild.GetUsersAsync();
            Dictionary<string,GuildUser> users = new Dictionary<string, GuildUser>();

            await foreach (var user in guildUsers)
            {
                users.Add(user.Username,user);
            }
            
            await foreach (RestMessage message in messages)
            {
                // Hylkää kaikki tyhjät ja bottien tekemät viestit listasta
                if (string.IsNullOrWhiteSpace(message.Content) || message.Author.IsBot)
                    continue;

                if (!users.TryGetValue(message.Author.Username, out var guildUser))
                    continue;
                  
                    lines.Add($"{guildUser.Nickname ?? message.Author.GlobalName}: {message.Content}");

                // Lisätään käyttäjän nimi jokaisen viestin alkuun AI:n tunnistettavaksi

                if (lines.Count >= messageLimit)
                    break;
            }
            lines.Reverse();
            return string.Join("\n", lines);
        }
    }
}
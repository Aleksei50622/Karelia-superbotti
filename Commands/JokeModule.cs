using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using System;
using System.Collections.Generic;
using System.Text;
using Superbotti.Services;


namespace Superbotti.Commands
{
    public class JokeModule(SpeechService speech, JokeService jokes) : ApplicationCommandModule<ApplicationCommandContext>
    {

        [SlashCommand("joke", "Tells a joke")]

        public async Task AIJoke(
            [SlashCommandParameter(Name = "subject", Description = "What should the joke be about?")]
            string jokeSubject = "Programmer dad joke")
        {
            await RespondAsync(InteractionCallback.DeferredMessage());

            try
            {
                string joke = await jokes.JokeAsync(jokeSubject);
                Stream audioResponse = await speech.TextToSpeechAsync(joke);
                var message = new InteractionMessageProperties().WithContent(joke).AddAttachments(new AttachmentProperties("joke.wav", audioResponse));

                // Kanavalle lähtevä viesti
                await FollowupAsync(message);
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
    }
}

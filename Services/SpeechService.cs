
namespace Superbotti.Services
{
    public class SpeechService(LocalAIClient localai, Models models)
    {
        /// <summary>
        /// Sends the given text to Local AI backend and returns it back as audio Stream
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<Stream> TextToSpeechAsync(string text)
            { 
            return localai.PostStreamAsync("/v1/audio/speech", new { model = models.SpeechModel, input = text }); 
        }
    }
}

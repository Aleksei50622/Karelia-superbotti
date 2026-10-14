
namespace Superbotti.Services
{
    public class SpeechService(LocalAIClient localai, Models models)
    {
        public Task<Stream> TextToSpeechAsync(string text)
            { 
            return localai.PostStreamAsync("/v1/audio/speech", new { model = models.SpeechModel, input = text }); 
        }
    }
}


using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Superbotti.Services
{
    public class SummaryService(LocalAIClient localai, Models models)
    {
        private const string prompt =
            "You summarize chat conversations. Make the summary at most few paragraphs." +
            "Try to use bullet points. Reply in the same language as the chat." +
            "Each message is in the format username: message.";

        public async Task<string> SummarizeAsync(string chat, CancellationToken ct = default)
        {
            var json = await localai.PostJsonAsync("/v1/chat/completions", new
            {
                model = models.TextModel,
                messages = new object[]
                {
                new { role = "system", content = prompt },
                new { role = "user", content = $"Summarize this conversation:\n\n{chat}" }
                }
            });

            return json["choices"][0]["message"]!["content"].GetValue<string>();
        }
    }
}

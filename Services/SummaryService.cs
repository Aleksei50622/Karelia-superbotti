
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Superbotti.Services
{
    public class SummaryService(LocalAIClient localai, Models models)
    {
        /// <summary>
        /// The system prompt that instructs the LLM how the summary is formed
        /// </summary>
        private const string prompt =
            "You summarize chat conversations. Make the summary at most few paragraphs." +
            "Reply in the same language as the chat." +
            "Text has to be shorter than the user prompt " +
            "Try to include the names of persons who are in the discussion.";

        /// <summary>
        /// Sends the summary request to LLM ánd returns the response as string
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Parses the text into separate paragraphs
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<string> ParseSummary(string text, CancellationToken ct = default)
        {
            var json = await localai.PostJsonAsync("/v1/chat/completions", new
            {
                model = models.TextModel,
                messages = new object[]
                {
                new { role = "user", content = $"Parse the into paragraphs: \n\n{text}" }
                }
            });

            return json["choices"][0]["message"]!["content"].GetValue<string>();
        }

    }
}

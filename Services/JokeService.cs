using System;
using System.Collections.Generic;
using System.Text;

namespace Superbotti.Services
{
    public class JokeService(LocalAIClient localai, Models models)
    {
        private const string prompt =
            "You tell Jokes. Tell the best joke you can come up with!";


        public async Task<string> JokeAsync(string? jokeSubject, CancellationToken ct = default)
        {
            var json = await localai.PostJsonAsync("/v1/chat/completions", new
            {
                model = models.TextModel,
                messages = new[]
        {
            new { role = "system", content = prompt },
            new { role = "user", content = $"Joke should be about: {jokeSubject}" }
        }
            });

            return json["choices"][0]["message"]!["content"].GetValue<string>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace Superbotti.Services
{
    public static class Capabilities
    {
        public const string Chat = "chat";
        public const string Speech = "tts";
    }
    public class Models(LocalAIClient localai)
    {
        // Pitäisi vaihtaa hakemaan aluksi appsettingsistä ja sen jälkeen vaihtumaan komennolla
        public string TextModel = "parable-granite-4.1-3b-claude-fable-5";
        public string AudioModel = "qwen3-tts-cpp";

        public Task<List<string>> GetChatModelsAsync()
        {
            return ListModelsAsync(Capabilities.Chat);
        }

        public Task<List<string>> GetSpeechModelsAsync()
        {
            return ListModelsAsync(Capabilities.Speech);
        }

        /// <summary>
        /// Returns a list of models with given capability
        /// </summary>
        /// <param name="capability"></param>
        /// <returns></returns>
        public async Task<List<string>> ListModelsAsync(string capability)
        {
            var json = await localai.GetJsonAsync("/v1/models/capabilities");
            var result = new List<string>();

            JsonArray? data = json["data"]?.AsArray();
            if (data is null)
                return result;

            // Tähän joku fiksumpi ratkaisu
            foreach (JsonNode? model in data)
            {
                string? id = model?["id"]?.GetValue<string>();
                JsonArray? capabilities = model?["capabilities"]?.AsArray();

                foreach (JsonNode? cap in capabilities)
                {
                    if (cap?.GetValue<string>() == capability)
                    {
                        result.Add(id);
                        break;
                    }
                }
            }
            return result;
        }
    }
}

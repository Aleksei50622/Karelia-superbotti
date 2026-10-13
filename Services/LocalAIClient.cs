using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace Superbotti.Services
{
    /// <summary>
    /// Contains most of the boilerplate for sending requests to LocalAI with httpClient
    /// </summary>
    /// <param name="httpClientFactory"></param>
    public class LocalAIClient(IHttpClientFactory httpClientFactory)
    {
        public const string HttpClientName = "LocalAI";
        private HttpClient CreateClient() => httpClientFactory.CreateClient(HttpClientName);

        public async Task<JsonNode> GetJsonAsync(string path)
        {
            var json = await CreateClient().GetFromJsonAsync<JsonNode>(path);
            return json;
        }

        public async Task<JsonNode> PostJsonAsync(string path, object body)
        {
            using var response = await SendAsync(path, body);
            var json = await response.Content.ReadFromJsonAsync<JsonNode>();
            return json;
        }

        public async Task<Stream> PostStreamAsync(string path, object body)
        {
            using var response = await SendAsync(path, body);

            var stream = new MemoryStream();
            await response.Content.CopyToAsync(stream);
            stream.Position = 0; // Ilman tätä alkaa streamin lopusta eli ei palauta mitään.
            return stream;
        }

        private async Task<HttpResponseMessage> SendAsync(string path, object body)
        {
            var response = await CreateClient().PostAsJsonAsync(path, body);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}

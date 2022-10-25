using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class SteamApiGamesAllGamesAppResponse
    {
        [JsonPropertyName("appid")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

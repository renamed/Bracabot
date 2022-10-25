using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class SteamApiGamesResponse
    {
        [JsonPropertyName("applist")]
        public SteamApiGamesAllGamesResponse AllApps { get; set; }
    }
}

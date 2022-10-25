using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class SteamApiGamesAllGamesResponse
    {
        [JsonPropertyName("apps")]
        public IEnumerable<SteamApiGamesAllGamesAppResponse> AllApps { get; set; }
    }
}

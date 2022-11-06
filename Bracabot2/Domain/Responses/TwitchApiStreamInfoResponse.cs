using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class TwitchApiStreamInfoResponse
    {
        [JsonPropertyName("data")]
        public List<TwitchApiStreamInfoNodeResponse> Data { get; set; }
    }
}

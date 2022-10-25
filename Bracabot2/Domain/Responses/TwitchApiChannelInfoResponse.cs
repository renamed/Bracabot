using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class TwitchApiChannelInfoResponse
    {
        [JsonPropertyName("data")]
        public IEnumerable<TwitchApiChannelInfoNodeResponse> Data { get; set; }
    }
}

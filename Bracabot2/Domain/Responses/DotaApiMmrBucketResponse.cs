using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiMmrBucketResponse
    {
        [JsonPropertyName("mmr")]
        public DotaApiMmrBucketRowsResponse Mmr { get; set; }
    }
}

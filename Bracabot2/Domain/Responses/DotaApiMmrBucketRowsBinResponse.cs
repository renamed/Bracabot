using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiMmrBucketRowsBinResponse
    {
        [JsonPropertyName("bin")]
        public int Bin { get; set; }

        [JsonPropertyName("bin_name")]
        public int BinName { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("cumulative_sum")]
        public int CumulativeSum { get; set; }

        [JsonPropertyName("loccountrycode")]
        public string Loccountrycode { get; set; }

        [JsonPropertyName("avg")]
        public string Avg { get; set; }

        [JsonPropertyName("common")]
        public string Common { get; set; }
    }
}

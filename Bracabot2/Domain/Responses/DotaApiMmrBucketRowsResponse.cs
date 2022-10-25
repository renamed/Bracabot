using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiMmrBucketRowsResponse
    {
        [JsonPropertyName("rows")]
        public List<DotaApiMmrBucketRowsBinResponse> Rows { get; set; }
    }
}

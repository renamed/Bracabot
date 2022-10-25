using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiPlayerResponse
    {
        [JsonPropertyName("leaderboard_rank")]
        public int? LeaderboardRank { get; set; }

        [JsonPropertyName("rank_tier")]
        public int RankTier { get; set; }
    }
}

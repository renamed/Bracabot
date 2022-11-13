using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiMatchResponse
    {
        [JsonPropertyName("match_id")]
        public long MatchId { get; set; }

        [JsonPropertyName("player_slot")]
        public int PlayerSlot { get; set; }

        [JsonPropertyName("radiant_win")]
        public bool RadiantWin { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("game_mode")]
        public int GameMode { get; set; }

        [JsonPropertyName("lobby_type")]
        public int LobbyType { get; set; }

        [JsonPropertyName("hero_id")]
        public int HeroId { get; set; }

        [JsonPropertyName("start_time")]
        public int UnixStartTime { get; set; }

        [JsonPropertyName("version")]
        public object Version { get; set; }

        [JsonPropertyName("kills")]
        public int Kills { get; set; }

        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }

        [JsonPropertyName("assists")]
        public int Assists { get; set; }

        [JsonPropertyName("average_rank")]
        public int? AverageRank { get; set; }

        [JsonPropertyName("leaver_status")]
        public int LeaverStatus { get; set; }

        public int? party_size { get; set; }

        public int PartySize { get => party_size ?? 1; }
    }
}

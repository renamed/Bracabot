﻿using System;
using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiRecentMatchResponse
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

        public DateTime StartTime => DateTimeOffset.FromUnixTimeSeconds(UnixStartTime).DateTime;
        public DateTime EndTime => DateTimeOffset.FromUnixTimeSeconds(UnixStartTime + Duration).DateTime;

        [JsonPropertyName("version")]
        public object Version { get; set; }

        [JsonPropertyName("kills")]
        public int Kills { get; set; }

        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }

        [JsonPropertyName("assists")]
        public int Assists { get; set; }

        [JsonPropertyName("skill")]
        public object Skill { get; set; }

        [JsonPropertyName("xp_per_min")]
        public int XpPerMin { get; set; }

        [JsonPropertyName("gold_per_min")]
        public int GoldPerMin { get; set; }

        [JsonPropertyName("hero_damage")]
        public int HeroDamage { get; set; }

        [JsonPropertyName("tower_damage")]
        public int TowerDamage { get; set; }

        [JsonPropertyName("hero_healing")]
        public int HeroHealing { get; set; }

        [JsonPropertyName("last_hits")]
        public int LastHits { get; set; }

        [JsonPropertyName("lane")]
        public object Lane { get; set; }

        [JsonPropertyName("lane_role")]
        public object LaneRole { get; set; }

        [JsonPropertyName("is_roaming")]
        public object IsRoaming { get; set; }

        [JsonPropertyName("cluster")]
        public int Cluster { get; set; }

        [JsonPropertyName("leaver_status")]
        public int LeaverStatus { get; set; }

        public int? party_size { get; set; }

        public int PartySize { get => party_size ?? 1; }
    }
}

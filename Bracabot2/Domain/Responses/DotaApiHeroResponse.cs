using System.Text.Json.Serialization;

namespace Bracabot2.Domain.Responses
{
    public class DotaApiHeroResponse
    {
        [JsonPropertyName("hero_id")]
        public string HeroId { get; set; }

        [JsonPropertyName("last_played")]
        public int LastPlayed { get; set; }

        [JsonPropertyName("games")]
        public int Games { get; set; }

        [JsonPropertyName("win")]
        public int Win { get; set; }

        public double WinRate => 1.0 * Win / Games;

        [JsonPropertyName("with_games")]
        public int WithGames { get; set; }

        [JsonPropertyName("with_win")]
        public int WithWin { get; set; }

        public double WithWinRate => 1.0 * WithWin / WithGames;

        [JsonPropertyName("against_games")]
        public int AgainstGames { get; set; }

        [JsonPropertyName("against_win")]
        public int AgainstWin { get; set; }

        public double AgainstWinRate => 1.0 * AgainstWin / AgainstGames;
    }
}

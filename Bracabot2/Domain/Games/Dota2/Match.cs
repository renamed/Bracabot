namespace Bracabot2.Domain.Games.Dota2
{
    public class Match
    {
        public Guid Id { get; set; }
        public long MatchId { get; set; }
        public MatchSlot PlayerSlot { get; set; }
        public MatchResult MatchResult { get; set; }
        public MatchType MatchType { get; set; }
        public int HeroId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int PartySize { get; set; }
    }
}

using Bracabot2.Domain.Responses;

namespace Bracabot2.Domain.Games.Dota2
{
    public class Dota2Statistics
    {
        public int Games { get; private set; }
        public int Victories { get; private set; }
        public int Defeats { get; private set; }
        
        public int AvgK {get; private set;}
        public int AvgD {get; private set;}
        public int AvgA { get; private set; }

        public bool HasError { get; private set; }
        public string ErrorDescription { get; private set; }

        public Dota2Statistics(IEnumerable<Match> eligibleMatches)
        {
            CompileInformation(eligibleMatches);
        }

        public void CompileInformation(IEnumerable<Match> eligibleMatches)
        {
            Games = eligibleMatches.Count();
            if (Games == 0)
            {
                SetError("Não achei partidas recentes");
                return;
            }

            Victories = eligibleMatches.Count(p => p.MatchResult == MatchResult.Win);
            Defeats = eligibleMatches.Count(p => p.MatchResult == MatchResult.Lose);
            if (Victories + Defeats != Games)
            {
                SetError("As contas do Renamede não estão corretas, avisa pra ele!");
                return;
            }

            AvgK = eligibleMatches.Sum(x => x.Kills) / Games;
            AvgD = eligibleMatches.Sum(x => x.Deaths) / Games;
            AvgA = eligibleMatches.Sum(x => x.Assists) / Games;
        }

        private void SetError(string error)
        {
            HasError = true;
            ErrorDescription = error;
        }
    }
}

using Bracabot2.Domain.Responses;

namespace Bracabot2.Domain.Games.Dota2
{
    public class Dota2Statistics
    {
        public int Games { get; private set; }
        public int Victories { get; private set; }
        public int Defeats { get; private set; }

        public int SoloVictories {get;private set;}
        public int PartyVictories { get; private set; }

        public int SoloDefeats {get; private set;}
        public int PartyDefeats { get; private set;}

        public int Mmr {get; private set;}

        public int AvgK {get; private set;}
        public int AvgD {get; private set;}
        public int AvgA { get; private set; }

        public bool HasError { get; private set; }
        public string ErrorDescription { get; private set; }

        public Dota2Statistics(IEnumerable<DotaApiRecentMatchResponse> eligibleMatches)
        {
            CompileInformation(eligibleMatches);
        }

        public void CompileInformation(IEnumerable<DotaApiRecentMatchResponse> eligibleMatches)
        {
            Games = eligibleMatches.Count();
            if (Games == 0)
            {
                SetError("Não achei partidas recentes");
                return;
            }


            Victories = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)));
            Defeats = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)));
            if (Victories + Defeats != Games)
            {
                SetError("As contas do Renamede não estão corretas, avisa pra ele!");
                return;
            }

            SoloVictories = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)) && p.PartySize == 1 && p.LobbyType == 7);
            PartyVictories = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)) && p.PartySize > 1 && p.LobbyType == 7);

            SoloDefeats = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)) && p.PartySize == 1 && p.LobbyType == 7);
            PartyDefeats = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)) && p.PartySize > 1 && p.LobbyType == 7);

            Mmr = 30 * SoloVictories - 30 * SoloDefeats + 20 * PartyVictories - 20 * PartyDefeats;

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

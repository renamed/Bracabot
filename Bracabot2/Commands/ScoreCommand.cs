using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using System.Text;

namespace Bracabot2.Commands
{
    public class ScoreCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;

        public ScoreCommand(IDotaService dotaService, ITwitchService twitchService)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaId = Environment.GetEnvironmentVariable("DOTA_ID");

            if (!await twitchService.EhOJogoDeDota())
            {
                return "Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.";
            }

            var response = await dotaService.GetRecentMatchesAsync(dotaId);
            if (response == default)
            {
                return "A API do Dota retornou um erro. Não consegui ver as últimas partidas";                
            }

            var eligibleMatches = new List<DotaApiRecentMatchResponse>();
            DotaApiRecentMatchResponse lastMatch = null;
            foreach (var currentMatch in response)
            {
                if (lastMatch == null)
                {
                    eligibleMatches.Add(currentMatch);
                    lastMatch = currentMatch;
                }
                else
                {
                    if (lastMatch.StartTime - currentMatch.StartTime >= TimeSpan.FromHours(5))
                    {
                        break;
                    }

                    eligibleMatches.Add(currentMatch);
                    lastMatch = currentMatch;
                }
            }

            if (DateTime.UtcNow - eligibleMatches.Max(x => x.StartTime) >= TimeSpan.FromHours(5))
            {
                return "Não achei partidas recentes";
            }

            var statistics = new Dota2Statistics(eligibleMatches);
            if (statistics.HasError)
            {
                return statistics.ErrorDescription;
            }
                        
            var sb = new StringBuilder();
            sb.Append($"J = {statistics.Games} --- V -> {(statistics.Victories != 0 ? statistics.Victories.ToString() : "Nenhuma")} --- D -> {(statistics.Defeats != 0 ? statistics.Defeats.ToString() : "Nenhuma")} ");
            sb.Append($"--- Saldo {statistics.Mmr:+#;-#;0}");

            return sb.ToString();
        }
    }
}

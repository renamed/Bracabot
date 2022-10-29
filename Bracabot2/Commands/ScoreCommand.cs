using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class ScoreCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;
        private readonly SettingsOptions options;

        public ScoreCommand(IDotaService dotaService, ITwitchService twitchService, IOptions<SettingsOptions> options)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
            this.options = options.Value;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaId = options.DotaId;

            if (!await twitchService.IsCurrentGameDota2())
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

            int qtdVitoriasSolo = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)) && p.PartySize == 1 && p.LobbyType == 7);
            int qtdVitoriasGrupo = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)) && p.PartySize > 1 && p.LobbyType == 7);

            int qtdDerrotasSolo = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)) && p.PartySize == 1 && p.LobbyType == 7);
            int qtdDerrotasGrupo = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)) && p.PartySize > 1 && p.LobbyType == 7);

            int qtdVitorias = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)));
            int qtdDerrotas = eligibleMatches.Count(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)));

            //int qtdVitoriasRecalibracao = recalibracaoMatches.Where(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin))).Count();
            //int qtdDerrotasRecalibracao = recalibracaoMatches.Where(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin))).Count();
            //int qtdRecalibracao = qtdVitoriasRecalibracao + qtdDerrotasRecalibracao;

            int mmr = 30 * qtdVitoriasSolo - 30 * qtdDerrotasSolo + 20 * qtdVitoriasGrupo - 20 * qtdDerrotasGrupo;

            if (qtdVitorias + qtdDerrotas != qtdJogos)
            {
                Console.WriteLine($"Jogos {qtdJogos} = V {qtdVitorias} = D {qtdDerrotas}");
                return "As contas do Renamede não estão corretas, avisa pra ele!";
            }

            var sb = new StringBuilder();
            sb.Append($"J = {statistics.Games} --- V -> {(statistics.Victories != 0 ? statistics.Victories.ToString() : "Nenhuma")} --- D -> {(statistics.Defeats != 0 ? statistics.Defeats.ToString() : "Nenhuma")} ");
            sb.Append($"--- Saldo {statistics.Mmr:+#;-#;0}");

            return sb.ToString();
        }
    }
}

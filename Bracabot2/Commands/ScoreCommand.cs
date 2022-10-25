using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var dotaService = new DotaService();
            var twitchService = new TwitchService();
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

            int qtdJogos = eligibleMatches.Count();
            if (qtdJogos == 0)
            {
                return "Não achei partidas recentes";
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
            sb.Append($"J = {qtdJogos} --- V -> {(qtdVitorias != 0 ? qtdVitorias.ToString() : "Nenhuma")} --- D -> {(qtdDerrotas != 0 ? qtdDerrotas.ToString() : "Nenhuma")} ");
            sb.Append($"--- Saldo {mmr:+#;-#;0}");
            //if (qtdRecalibracao != 0)
            //{
            //    sb.Append($" --- RECALIBRAÇÃO ==> V -> {(qtdVitoriasRecalibracao != 0 ? qtdVitoriasRecalibracao.ToString() : "Nenhuma")} -- D -> {(qtdDerrotasRecalibracao != 0 ? qtdDerrotasRecalibracao.ToString() : "Nenhuma")}");
            //}

            return sb.ToString();
        }
    }
}

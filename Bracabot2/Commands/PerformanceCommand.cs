using Bracabot2.Domain.Responses;
using Bracabot2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class PerformanceCommand : ICommand
    {
        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaService = new DotaService();
            var twitchService = new TwitchService();
            var dotaId = Environment.GetEnvironmentVariable("DOTA_ID");

            if (!await twitchService.EhOJogoDeDota())
            {
                return "Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.";
            }

            var eligibleMatches = await dotaService.GetRecentMatchesAsync(dotaId);
            if (eligibleMatches == default)
            {
                return "A API do Dota retornou um erro. Não consegui ver as últimas partidas";                
            }
            int qtdJogos = eligibleMatches.Count();

            int qtdVitoriasSolo = eligibleMatches.Where(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)) && p.PartySize == 1 && p.LobbyType == 7).Count();
            int qtdVitoriasGrupo = eligibleMatches.Where(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin)) && p.PartySize > 1 && p.LobbyType == 7).Count();

            int qtdDerrotasSolo = eligibleMatches.Where(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)) && p.PartySize == 1 && p.LobbyType == 7).Count();
            int qtdDerrotasGrupo = eligibleMatches.Where(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin)) && p.PartySize > 1 && p.LobbyType == 7).Count();

            int qtdVitorias = eligibleMatches.Where(p => ((p.PlayerSlot < 100 && p.RadiantWin) || (p.PlayerSlot >= 100 && !p.RadiantWin))).Count();
            int qtdDerrotas = eligibleMatches.Where(p => ((p.PlayerSlot < 100 && !p.RadiantWin) || (p.PlayerSlot >= 100 && p.RadiantWin))).Count();

            int mmr = 30 * qtdVitoriasSolo - 30 * qtdDerrotasSolo + 20 * qtdVitoriasGrupo - 20 * qtdDerrotasGrupo;

            if (qtdVitorias + qtdDerrotas != qtdJogos)
            {
                Console.WriteLine($"Jogos {qtdJogos} = V {qtdVitorias} = D {qtdDerrotas}");
                return "As contas do Renamede não estão corretas, avisa pra ele!";                
            }

            var sb = new StringBuilder();
            sb.Append($"J = {qtdJogos} --- V -> {(qtdVitorias != 0 ? qtdVitorias.ToString() : "Nenhuma")} --- D -> {(qtdDerrotas != 0 ? qtdDerrotas.ToString() : "Nenhuma")} ");
            sb.Append($"--- Saldo {mmr:+#;-#;0}");

            int mediaK = eligibleMatches.Sum(x => x.Kills) / qtdJogos;
            int mediaD = eligibleMatches.Sum(x => x.Deaths) / qtdJogos;
            int mediaA = eligibleMatches.Sum(x => x.Assists) / qtdJogos;
            sb.Append($" --- Média (K/D/A) ({mediaK}/{mediaD}/{mediaA}).");

            IEnumerable<IGrouping<int, DotaApiRecentMatchResponse>> heroIdMaisJogado = eligibleMatches.GroupBy(x => x.HeroId);
            IGrouping<int, DotaApiRecentMatchResponse> qtd = heroIdMaisJogado.OrderByDescending(x => x.Count()).First();

            var nomeHeroMaisJogado = dotaService.GetNameAsync(qtd.Key.ToString());

            sb.Append($" --- {heroIdMaisJogado.Count()} heroi(s) único(s) --- Heroi mais jogado: {nomeHeroMaisJogado} com {qtd.Count()} vez(es) ({1.0 * qtd.Count() / qtdJogos:P1}).");

            return sb.ToString();
        }
    }
}

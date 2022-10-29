using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using System.Text;

namespace Bracabot2.Commands
{
    public class PerformanceCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;
        private readonly SettingsOptions options;

        public PerformanceCommand(IDotaService dotaService, ITwitchService twitchService, IOptions<SettingsOptions> options)
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

            var eligibleMatches = await dotaService.GetRecentMatchesAsync(dotaId);
            if (eligibleMatches == default)
            {
                return "A API do Dota retornou um erro. Não consegui ver as últimas partidas";                
            }
            int qtdJogos = eligibleMatches.Count();

            var statistics = new Dota2Statistics(eligibleMatches);
            if (statistics.HasError)
            {
                return statistics.ErrorDescription;
            }

            var sb = new StringBuilder();
            sb.Append($"J = {statistics.Games} --- V -> {(statistics.Victories != 0 ? statistics.Victories.ToString() : "Nenhuma")} --- D -> {(statistics.Defeats != 0 ? statistics.Defeats.ToString() : "Nenhuma")} ");
            sb.Append($"--- Saldo {statistics.Mmr:+#;-#;0}");

            sb.Append($" --- Média (K/D/A) ({statistics.AvgK}/{statistics.AvgD}/{statistics.AvgA}).");

            IEnumerable<IGrouping<int, DotaApiRecentMatchResponse>> heroIdMaisJogado = eligibleMatches.GroupBy(x => x.HeroId);
            IGrouping<int, DotaApiRecentMatchResponse> qtd = heroIdMaisJogado.OrderByDescending(x => x.Count()).First();

            var nomeHeroMaisJogado = await dotaService.GetNameAsync(qtd.Key.ToString());

            sb.Append($" --- {heroIdMaisJogado.Count()} heroi(s) único(s) --- Heroi mais jogado: {nomeHeroMaisJogado} com {qtd.Count()} vez(es) ({1.0 * qtd.Count() / statistics.Games:P1}).");

            return sb.ToString();
        }
    }
}

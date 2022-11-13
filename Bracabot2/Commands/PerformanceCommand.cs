﻿using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using System.Text;

namespace Bracabot2.Commands
{
    public class PerformanceCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;
        private readonly IDotaRepository dotaRepository;

        public PerformanceCommand(IDotaService dotaService, ITwitchService twitchService, IDotaRepository dotaRepository)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
            this.dotaRepository = dotaRepository;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var streamInfo = await twitchService.GetStreamInfo();
            if (streamInfo == default)
            {
                return "Streamer não está online";
            }

            if (!streamInfo.IsDota2Game)
            {
                return "Comando só disponível quando o streamer estiver jogando o jogo de Dota. !dota tem todas as informações.";
            }
            
            var eligibleMatches = await dotaRepository.GetLastMatchesAsync(streamInfo.StartedAt);
            if (eligibleMatches == default)
            {
                return "Nenhuma partida encontrada. Seria essa a primeira? ;)";
            }            
            
            if (!eligibleMatches.Any())
            {
                return "Nenhuma partida encontrada. Seria essa a primeira? ;)";
            }
                        
            var statistics = new Dota2Statistics(eligibleMatches);
            if (statistics.HasError)
            {
                return statistics.ErrorDescription;
            }

            var sb = new StringBuilder();
            sb.Append($"J = {statistics.Games} --- V -> {(statistics.Victories != 0 ? statistics.Victories.ToString() : "Nenhuma")} --- D -> {(statistics.Defeats != 0 ? statistics.Defeats.ToString() : "Nenhuma")} ");
            sb.Append($"--- Saldo {statistics.Mmr:+#;-#;0}");

            sb.Append($" --- Média (K/D/A) ({statistics.AvgK}/{statistics.AvgD}/{statistics.AvgA}).");

            IEnumerable<IGrouping<int, Match>> mostPlayedHeroes = eligibleMatches.GroupBy(x => x.HeroId);
            IGrouping<int, Match> mostPlayedHeroWithGameCount = mostPlayedHeroes.OrderByDescending(x => x.Count()).First();

            var mostPlayedHeroName = await dotaService.GetNameAsync(mostPlayedHeroWithGameCount.Key.ToString());

            sb.Append($" --- {mostPlayedHeroes.Count()} heroi(s) único(s) --- Heroi mais jogado: {mostPlayedHeroName} com {mostPlayedHeroWithGameCount.Count()} vez(es) ({1.0 * mostPlayedHeroWithGameCount.Count() / statistics.Games:P1}).");

            return sb.ToString();
        }
    }
}

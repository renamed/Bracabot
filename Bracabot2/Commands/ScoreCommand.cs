using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using System.Text;

namespace Bracabot2.Commands
{
    public class ScoreCommand : ICommand
    {
        private readonly ITwitchService twitchService;        
        private readonly IDotaRepository dotaRepository;

        public ScoreCommand(ITwitchService twitchService, IDotaRepository dotaRepository)
        {
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
                return "Nenhuma partida encontrada. Seria essa a primeira do dia? ;)";
            }

            if (!eligibleMatches.Any())
            {
                return "Nenhuma partida encontrada. Seria essa a primeira do dia? ;)";
            }

            var statistics = new Dota2Statistics(eligibleMatches);
            if (statistics.HasError)
            {
                return statistics.ErrorDescription;
            }

            return $"J = {statistics.Games} --- V -> {(statistics.Victories != 0 ? statistics.Victories.ToString() : "Nenhuma")} --- D -> {(statistics.Defeats != 0 ? statistics.Defeats.ToString() : "Nenhuma")}";                        
        }
    }
}

using Bracabot2.Domain.Interfaces;

namespace Bracabot2.Commands
{
    public class MedalCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;

        public MedalCommand(IDotaService dotaService, ITwitchService twitchService)
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

            var player = await dotaService.GetPlayerAsync(dotaId);

            int slot = player.RankTier / 10;
            var medal = await dotaService.GetMedalAsync(slot);
            if (medal == null)
            {
                return "Deu bug, não encontrei a medalha informada pela Valve =/";
            }            

            if (slot == 8)
            {
                return $"{medal} rank {player.LeaderboardRank}";
            }

            if (player.LeaderboardRank != null)
            {
                return $"Rank {player.LeaderboardRank}";
            }
            
            int stars = player.RankTier % 10;
            return $"{medal} {stars}";
        }
    }
}

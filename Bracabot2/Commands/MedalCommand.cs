using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;

namespace Bracabot2.Commands
{
    public class MedalCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private readonly ITwitchService twitchService;
        private readonly SettingsOptions options;

        public MedalCommand(IDotaService dotaService, ITwitchService twitchService, IOptions<SettingsOptions> options)
        {
            this.dotaService = dotaService;
            this.twitchService = twitchService;
            this.options = options.Value;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaId = options.DotaId;
            var streamInfo = await twitchService.GetStreamInfo();
            if (streamInfo == default)
            {
                return "Streamer não está online";
            }

            if (!streamInfo.IsDota2Game)
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


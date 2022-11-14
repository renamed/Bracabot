using Bracabot2.Domain.Extensions;
using Bracabot2.Domain.Interfaces;
using System.Text;

namespace Bracabot2.Commands
{
    public class FreedomCommand : ICommand
    {
        private readonly IDotaRepository dotaRepository;
        private readonly ITwitchService twitchService;

        public FreedomCommand(IDotaRepository dotaRepository, ITwitchService twitchService)
        {
            this.dotaRepository = dotaRepository;
            this.twitchService = twitchService;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var streamInfo = await twitchService.GetStreamInfo();
            if (streamInfo == default)
            {
                return "Streamer não está online";
            }

            if (streamInfo.IsDota2Game)
            {
                return "O jogo de Dota 2 está rolando agora :D";
            }

            var lastMatch = await dotaRepository.GetLastMatchAsync();
            if (lastMatch == default)            
                return "Não encontrei partidas recentes do jogo de Dota 2";            

            var freedomTime = DateTime.UtcNow - lastMatch.EndTime;

            var sb = new StringBuilder("Estamos há ");
            sb.Append(freedomTime.GetReadable());
            sb.Append($" sem o jogo de Dota bracubiClap");

            return sb.ToString();
        }
    }
}

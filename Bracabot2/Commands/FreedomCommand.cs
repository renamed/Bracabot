using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using System.Text;

namespace Bracabot2.Commands
{
    public class FreedomCommand : ICommand
    {
        private readonly IDotaService dotaService;

        public FreedomCommand(IDotaService dotaService)
        {
            this.dotaService = dotaService;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {            
            var dotaId = Environment.GetEnvironmentVariable("DOTA_ID");

            IEnumerable<DotaApiRecentMatchResponse> response = await dotaService.GetRecentMatchesAsync(dotaId);
            if (response == default)            
                return "A API do Dota retornou um erro. Não consegui ver as últimas partidas";

            var freedomTime = DateTime.UtcNow - response.FirstOrDefault().EndTime;


            var sb = new StringBuilder("Estamos há ");
            sb.Append(freedomTime.GetReadable());
            sb.Append($" sem o jogo de Dota bracubiClap");

            return sb.ToString();
        }
    }
}

using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Responses;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class FreedomCommand : ICommand
    {
        private readonly IDotaService dotaService;
        private SettingsOptions options;

        public FreedomCommand(IDotaService dotaService,
            IOptions<SettingsOptions> options)
        {
            this.dotaService = dotaService;
            this.options = options.Value;
        }

        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaId = options.DotaId;
            
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

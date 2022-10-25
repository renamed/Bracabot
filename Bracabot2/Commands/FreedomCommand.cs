using Bracabot2.Domain.Responses;
using Bracabot2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class FreedomCommand : ICommand
    {
        public async Task<string> ExecuteAsync(string[] args)
        {
            var dotaService = new DotaService();
            var dotaId = Environment.GetEnvironmentVariable("DOTA_ID");

            IEnumerable<DotaApiRecentMatchResponse> response = await dotaService.GetRecentMatchesAsync(dotaId);
            if (response == default)            
                return "A API do Dota retornou um erro. Não consegui ver as últimas partidas";

            var freedomTime = DateTime.UtcNow - response.FirstOrDefault().EndTime;
            var sb = new StringBuilder("Estamos há");
            if (freedomTime.TotalDays >= 7)
            {
                sb.Append($" {Convert.ToInt32(freedomTime.TotalDays / 7)} semanas,");
            }
            if (freedomTime.Days >= 1)
            {
                sb.Append($" {freedomTime.Days} dia(s),");
            }
            if (freedomTime.Hours >= 1)
            {
                sb.Append($" {freedomTime.Hours} hora(s),");
            }
            if (freedomTime.Minutes >= 1)
            {
                sb.Append($" {freedomTime.Minutes} minuto(s),");
            }
            sb.Append($" {freedomTime.Seconds} segundo(s) sem o jogo de Dota bracubiClap");
            return sb.ToString();
        }
    }
}

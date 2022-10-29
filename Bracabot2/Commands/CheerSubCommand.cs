using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class CheerSubCommand : ICommand
    {
        private static DateTime ultimoToca = DateTime.MinValue;

        public Task<string> ExecuteAsync(string[] args)
        {
            //if (!line.Contains("mod=1"))
            //{
            var diff = DateTime.UtcNow - ultimoToca;
            if (diff <= TimeSpan.FromMinutes(2))
            {
                var diffRestante = TimeSpan.FromMinutes(2) - diff;
                string tempo;
                if (diffRestante < TimeSpan.FromSeconds(60))
                {
                    tempo = $"{diffRestante:ss} segundos";
                }
                else if (diffRestante == TimeSpan.FromSeconds(60))
                {
                    tempo = $"{diffRestante:mm} minuto";
                }
                else
                {
                    tempo = $"{diffRestante:mm} minuto e {diffRestante:ss} segundo(s)";
                }

                return Task.FromResult($"Aguarde mais {tempo} para usar o comando !toca");
            }

            ultimoToca = DateTime.UtcNow;
            return Task.FromResult("!kappagen bracubiTrombeta ");
        }
    }
}

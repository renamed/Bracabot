using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class RecalibrationCommand : ICommand
    {
        public Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult("Terminei de recarregar em 22/10/2022 com 4 Vitórias e 6 Derrotas. Medalha alcançada: Cruzado 4");
        }
    }
}

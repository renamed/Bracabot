using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class CommandsCommand : ICommand
    {
        public Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult("!aproveitamento !placar !heroi !medalha !tai !comandos !histograma !info !liberdade !recalibracao");
        }
    }
}

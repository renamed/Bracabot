using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public class PingCommand : ICommand
    {
        public Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult("Sim!");
        }
    }
}

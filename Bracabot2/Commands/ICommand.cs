using System.Threading.Tasks;

namespace Bracabot2.Commands
{
    public interface ICommand
    {
        Task<string> ExecuteAsync(string[] args);
    }
}

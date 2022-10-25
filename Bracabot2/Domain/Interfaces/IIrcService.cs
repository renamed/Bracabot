using System.IO;
using System.Threading.Tasks;

namespace Bracabot2.Domain.Interfaces
{
    public interface IIrcService
    {
        public StreamReader Reader { get; }
        public StreamWriter Writer { get; }

        Task ConnectAsync();
        Task<string> GetMessageAsync();
        Task SendMessageAsync(string channelName, string message);
        Task SendPongAsync(string pongParam);
    }
}

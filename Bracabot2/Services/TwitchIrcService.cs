using Bracabot2.Domain.Interfaces;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Bracabot2.Services
{
    public class TwitchIrcService : IDisposable, IIrcService
    {
        public TcpClient TcpClient { get; private set; }
        public StreamReader Reader { get; private set; }
        public StreamWriter Writer { get; private set; }

        public async Task ConnectAsync()
        {
            string ip = Environment.GetEnvironmentVariable("IP_TWITCH_IRC");
            int port = Convert.ToInt32(Environment.GetEnvironmentVariable("PORT_TWITCH_IRC"));
            string password = Environment.GetEnvironmentVariable("PASSWORD_TWITCH_IRC");
            string username = Environment.GetEnvironmentVariable("USERNAME_TWITCH_IRC");
            string nomeCanal = Environment.GetEnvironmentVariable("CHANNEL_NAME");

            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);

            Reader = new StreamReader(tcpClient.GetStream());
            Writer = new StreamWriter(tcpClient.GetStream()) { NewLine = "\r\n", AutoFlush = true };

            await Writer.WriteLineAsync($"PASS {password}");
            await Writer.WriteLineAsync($"NICK {username}");
            await Writer.WriteLineAsync($"JOIN #{nomeCanal}");

            await Writer.WriteLineAsync("CAP REQ :twitch.tv/tags");
        }

        public async Task<string> GetMessageAsync()
        {
            return await Reader.ReadLineAsync();
        }

        public Task SendPongAsync(string pongParam)
        {
            return Writer.WriteLineAsync($"PONG {pongParam}");
        }

        public async Task SendMessageAsync(string channelName, string message)
        {
            await Writer.WriteLineAsync($"PRIVMSG #{channelName} :{message}");
        }

        
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Reader.Dispose();
            Writer.Dispose();
            TcpClient.Dispose();
        }
    }
}

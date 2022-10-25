using System.IO;
using System.Net.Sockets;

namespace Bracabot2.Domain
{
    public class TwitchIrcConnection
    {
        public TcpClient TcpClient { get; internal set; }
        public StreamReader Reader { get; internal set; }
        public StreamWriter Writer { get; internal set; }
    }
}

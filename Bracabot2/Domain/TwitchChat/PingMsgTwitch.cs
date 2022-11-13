using Bracabot2.Domain.Interfaces;

namespace Bracabot2.Domain.TwitchChat
{
    public class PingMsgTwitch : ITwitchMessage
    {
        public bool IsPing => true;
        public string Message { get; set; }
    }
}

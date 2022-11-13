using Bracabot2.Domain.Interfaces;

namespace Bracabot2.Domain.TwitchChat
{
    public class PrivMsgTwitch : ITwitchMessage
    {
        public bool IsPing => false;
        public DateTime DateTime { get; set; }        
        public Sender Sender { get; set; }        
        public Receiver Receiver { get; set; }
    }
}

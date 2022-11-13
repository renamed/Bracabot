using Bracabot2.Domain.TwitchChat;

namespace Bracabot2.Domain.Interfaces
{
    public interface ITwitchMessageHandler
    {
        Task<string> Handle(PingMsgTwitch pingMsg);
        Task<string> Handle(PrivMsgTwitch msg);

        Task<string> Handle(ITwitchMessage msg);
    }
}

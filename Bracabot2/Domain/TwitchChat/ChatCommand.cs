namespace Bracabot2.Domain.TwitchChat
{
    public class ChatCommand
    {
        public string Command { get; set; }
        public IEnumerable<string> Args { get; set; }
    }
}

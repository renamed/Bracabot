namespace Bracabot2.Domain.TwitchChat
{
    public class Sender
    {
        public string UserId { get; set; }
        public string UserDisplayNick { get; set; }
        public Role Role { get; set; }
        public bool? HasPrime { get; set; }
    }
}

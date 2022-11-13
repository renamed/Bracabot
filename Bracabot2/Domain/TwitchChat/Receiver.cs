namespace Bracabot2.Domain.TwitchChat
{
    public class Receiver
    {
        public string Room { get; set; }
        public string Message { get; set; }
        public string RoomId { get; set; }

        public ChatCommand GetChatCommand()
        {
            var chatMessage = Message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return new ChatCommand
            {
                Command = chatMessage.First().Trim(),
                Args = chatMessage.Skip(1)
            };
        }
    }
}

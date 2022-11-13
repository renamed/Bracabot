using Bracabot2.Domain.Interfaces;

namespace Bracabot2.Domain.TwitchChat.Parsers
{
    public static class TwitchMessageParser
    {
        public static ITwitchMessage Parse(string message)
        {
            if (message == null)
                return null;

            if (message.StartsWith("PING"))
                return ParsePing(message);
            if (message.Contains("PRIVMSG"))
                return ParsePrivMsg(message);

            return null;
        }

        private static PingMsgTwitch ParsePing(string message)
        {
            string[] split = message.Split(" ");
            return new PingMsgTwitch
            {
                Message = split[1]
            };
        }

        private static PrivMsgTwitch ParsePrivMsg(string message)
        {
            var tokens = message.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var metadata = tokens[0].Split(';', StringSplitOptions.RemoveEmptyEntries);

            return new PrivMsgTwitch
            {
                DateTime = GetMessageTimespan(metadata),
                Receiver = new Receiver
                {
                    Message = GetChatMessage(tokens),
                    Room = GetRoom(tokens),
                    RoomId = GetRoomId(metadata)
                },
                Sender = new Sender
                {
                    HasPrime = HasPrime(metadata),
                    Role = GetRole(metadata),
                    UserDisplayNick = GetUserDisplayNick(metadata),
                    UserId = GetUserId(metadata)
                }
            };
        }

        private static Role GetRole(IEnumerable<string> tokens)
        {
            var badge = tokens.FirstOrDefault(x => x.StartsWith("badges"));
            if (badge.Contains("broadcaster"))
                return Role.Owner;
            
            var mod = tokens.FirstOrDefault(x => x.Trim() == "mod=1");
            if (mod != null)
                return Role.Mod;

            var vip = tokens.FirstOrDefault(x => x.Trim() == "vip=1");
            if (vip != null)
                return Role.Vip;

            return Role.Regular;
        }

        private static DateTime GetMessageTimespan(IEnumerable<string> tokens)
        {
            var messageTimespan = tokens.FirstOrDefault(x => x.StartsWith("tmi-sent-ts"));
            var unixTimespanMs = Convert.ToInt64(messageTimespan.Split("=").Last());
            return new DateTime(DateTimeOffset.FromUnixTimeMilliseconds(unixTimespanMs).UtcTicks, DateTimeKind.Utc);
        }

        private static string GetChatMessage(IEnumerable<string> tokens)
        {
            return tokens.Last();
        }

        private static string GetRoom(IEnumerable<string> tokens)
        {
            var currentToken = tokens.ElementAt(1);
            var indexOf = currentToken.LastIndexOf('#') + 1;
            var channel = currentToken.Skip(indexOf).TakeWhile(x => x != ':');
            return string.Join(string.Empty, channel).Trim();
        }

        private static bool? HasPrime(IEnumerable<string> tokens)
        {
            var badge = tokens.FirstOrDefault(x => x.StartsWith("badges"));
            return badge.Contains("premium") ? true : null;
        }
        
        private static string GetUserDisplayNick(IEnumerable<string> tokens)
        {
            var userDisplayNick = tokens.FirstOrDefault(x => x.StartsWith("display-name"));
            return userDisplayNick.Split('=', StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private static string GetUserId(IEnumerable<string> tokens)
        {
            var userId = tokens.FirstOrDefault(x => x.StartsWith("user-id"));
            return userId.Split('=', StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private static string GetRoomId(IEnumerable<string> tokens)
        {
            var roomId = tokens.FirstOrDefault(x => x.StartsWith("room-id"));
            return roomId.Split('=', StringSplitOptions.RemoveEmptyEntries).Last();
        }
    }
}
    


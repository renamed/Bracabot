using Bracabot2.Domain.TwitchChat;
using Bracabot2.Domain.TwitchChat.Parsers;
using System;
using Xunit;

namespace Bracabot.UnitTests.Domain.TwitchChat.Parsers
{
    public class TwitchMessageParserUnitTest
    {
        private const string PingMessage = "PING :tmi.twitch.tv";
        private const string BroadcasterMessage = "@badge-info=subscriber/9;badges=broadcaster/1,subscriber/3009,premium/1;color=;display-name=RenaMede;emotes=;first-msg=0;flags=;id=284b94f3-827f-4c8d-b168-1d0019cfa196;mod=0;returning-chatter=0;room-id=669591094;subscriber=1;tmi-sent-ts=1668346482496;turbo=0;user-id=669591094;user-type= :renamede!renamede@renamede.tmi.twitch.tv PRIVMSG #renamede :oi";
        private const string RegularMessage = "@badge-info=;badges=premium/1;client-nonce=9fa3ee18652289a14f24cc630f6a5794;color=;display-name=RenaMede;emotes=;first-msg=0;flags=;id=29a5c156-1fd8-4102-b984-eda98b172753;mod=0;returning-chatter=0;room-id=76382514;subscriber=0;tmi-sent-ts=1668347515568;turbo=0;user-id=669591094;user-type= :renamede!renamede@renamede.tmi.twitch.tv PRIVMSG #aculho :ehehe";
        private const string ModMessage = "@badge-info=subscriber/17;badges=moderator/1,subscriber/12,sub-gift-leader/1;color=;display-name=RenaMede;emotes=;first-msg=0;flags=;id=6b9a44ad-2c5a-4d74-bff2-be54076d49f5;mod=1;returning-chatter=0;room-id=74510447;subscriber=1;tmi-sent-ts=1668347583556;turbo=0;user-id=669591094;user-type=mod :renamede!renamede@renamede.tmi.twitch.tv PRIVMSG #bracubi :TESTE";
        private const string VipSubMessage = "@badge-info=subscriber/8;badges=vip/1,subscriber/6,premium/1;color=;display-name=RenaMede;emotes=;first-msg=0;flags=;id=7eb22c9a-b783-401c-9228-5e4c41ca35eb;mod=0;returning-chatter=0;room-id=120081011;subscriber=1;tmi-sent-ts=1668347695225;turbo=0;user-id=669591094;user-type=;vip=1 :renamede!renamede@renamede.tmi.twitch.tv PRIVMSG #deabraba :teste 2";
        private const string VipNoSubMessage = "@badge-info=;badges=vip/1,premium/1;color=;display-name=RenaMede;emotes=;first-msg=0;flags=;id=91ce71a5-2bcb-46c7-8d39-23cc9fd055c6;mod=0;returning-chatter=0;room-id=205500243;subscriber=0;tmi-sent-ts=1668348087804;turbo=0;user-id=669591094;user-type=;vip=1 :renamede!renamede@renamede.tmi.twitch.tv PRIVMSG #gusttavindota :teste 2";

        [Fact]
        public void Parse_ShouldReturnNull_WhenMessageIsNull()
        {
            // Arrange
            string message = null;

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Null(parsed);
        }

        [Fact]
        public void Parse_ShouldParseMessage_WhenMessageIsPing()
        {
            // Arrange
            string message = PingMessage;

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(":tmi.twitch.tv", parsed.Receiver.Message);
        }

        [Fact]
        public void Parse_ShouldParsePing_WhenMessageIsPing()
        {
            // Arrange
            string message = PingMessage;

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.True(parsed.IsPing);
        }

        [Theory]
        [InlineData(BroadcasterMessage)]
        [InlineData(RegularMessage)]
        [InlineData(ModMessage)]
        [InlineData(VipSubMessage)]
        [InlineData(VipNoSubMessage)]

        public void Parse_ShouldParsePingFalse_WhenMessageIsNotPing(string message)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.False(parsed.IsPing);
        }

        [Theory]
        [InlineData(BroadcasterMessage, 638039432824960000)]
        [InlineData(RegularMessage, 638039443155680000)]
        [InlineData(ModMessage, 638039443835560000)]
        [InlineData(VipSubMessage, 638039444952250000)]
        [InlineData(VipNoSubMessage, 638039448878040000)]
        public void Parse_ShouldParseDateMessage(string message, long ticks)
        {
            // Arrange
            var expected = new DateTime(ticks);

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(expected, parsed.DateTime);
            Assert.Equal(DateTimeKind.Utc, parsed.DateTime.Kind);
        }

        [Theory]
        [InlineData(BroadcasterMessage, "oi")]
        [InlineData(RegularMessage, "ehehe")]
        [InlineData(ModMessage, "TESTE")]
        [InlineData(VipSubMessage, "teste 2")]
        [InlineData(VipNoSubMessage, "teste 2")]
        public void Parse_ShouldParseChatMessage(string message, string chatMessage)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(chatMessage, parsed.Receiver.Message);
        }

        [Theory]
        [InlineData(BroadcasterMessage, "renamede")]
        [InlineData(RegularMessage, "aculho")]
        [InlineData(ModMessage, "bracubi")]
        [InlineData(VipSubMessage, "deabraba")]
        [InlineData(VipNoSubMessage, "gusttavindota")]
        public void Parse_ShouldParseRoom(string message, string room)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(room, parsed.Receiver.Room);
        }

        [Theory]
        [InlineData(BroadcasterMessage, "669591094")]
        [InlineData(RegularMessage, "76382514")]
        [InlineData(ModMessage, "74510447")]
        [InlineData(VipSubMessage, "120081011")]
        [InlineData(VipNoSubMessage, "205500243")]
        public void Parse_ShouldParseRoomId(string message, string room)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(room, parsed.Receiver.RoomId);
        }

        [Theory]
        [InlineData(BroadcasterMessage, true)]
        [InlineData(RegularMessage, true)]
        [InlineData(ModMessage, null)]
        [InlineData(VipSubMessage, true)]
        [InlineData(VipNoSubMessage, true)]
        public void Parse_ShouldParsePrime(string message, bool? result)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(result, parsed.Sender.HasPrime);
        }

        [Theory]
        [InlineData(BroadcasterMessage)]
        [InlineData(RegularMessage)]
        [InlineData(ModMessage)]
        [InlineData(VipSubMessage)]
        [InlineData(VipNoSubMessage)]
        public void Parse_ShouldParseUserDisplayNick(string message)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal("RenaMede", parsed.Sender.UserDisplayNick);
        }

        [Theory]
        [InlineData(BroadcasterMessage)]
        [InlineData(RegularMessage)]
        [InlineData(ModMessage)]
        [InlineData(VipSubMessage)]
        [InlineData(VipNoSubMessage)]
        public void Parse_ShouldParseUserId(string message)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal("669591094", parsed.Sender.UserId);
        }

        [Theory]
        [InlineData(BroadcasterMessage, Role.Owner)]
        [InlineData(RegularMessage, Role.Regular)]
        [InlineData(ModMessage, Role.Mod)]
        [InlineData(VipSubMessage, Role.Vip)]
        [InlineData(VipNoSubMessage, Role.Vip)]
        public void Parse_ShouldParseRole(string message, Role expected)
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse(message);

            // Assert
            Assert.Equal(expected, parsed.Sender.Role);
        }

        [Fact]
        public void Parse_ShouldReturnNull_WhenMessageIsNotPingOrPrivMsg()
        {
            // Arrange

            // Act
            var parsed = TwitchMessageParser.Parse("bagaga");

            // Assert
            Assert.Null(parsed);
        }
    }
}

using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.TwitchChat;
using Serilog;
using System.Diagnostics;

namespace Bracabot2.Services
{
    public class TwitchMessageHandler : ITwitchMessageHandler
    {
        private readonly CommandFactory commandFactory;
        private readonly ILogger logger;

        public TwitchMessageHandler(CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
            logger = Log.ForContext<TwitchMessageHandler>();
        }

        public Task<string> Handle(PingMsgTwitch pingMsg)
        {
            return Task.FromResult($"PONG {pingMsg.Message}");
        }

        public async Task<string> Handle(PrivMsgTwitch msg)
        {
            var watch = new Stopwatch();

            var chatCommand = msg.Receiver.GetChatCommand();
            var command = commandFactory.Get(chatCommand.Command);
            if (command == null) return null;

            watch.Restart();
            var message = await command.ExecuteAsync(chatCommand.Args?.ToArray());
            watch.Stop();

            logger.Information("Command {0} executed in {1} ms", chatCommand.Command, watch.ElapsedMilliseconds);
            
            return $"PRIVMSG #{msg.Receiver.Room} :{message}";
        }

        public async Task<string> Handle(ITwitchMessage msg)
        {
            if (msg is PrivMsgTwitch msgTwitch)
                return await Handle(msgTwitch);
            if (msg is PingMsgTwitch pingMsgTwitch)
                return await Handle(pingMsgTwitch);
            return null;
        }
    }
}

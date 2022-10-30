using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;

namespace Bracabot2.Services
{
    public class TwitchFacade : IBotFacade
    {
        public volatile bool ShouldStop;

        private readonly CommandFactory commandFactory;
        private readonly ILogger logger;
        private readonly SettingsOptions options;
        private readonly IIrcService twitchIrcService;

        public TwitchFacade(CommandFactory commandFactory
            , IOptions<SettingsOptions> options, IIrcService twitchIrcService)
        {
            this.commandFactory = commandFactory;
            this.options = options.Value;            
            this.twitchIrcService = twitchIrcService;

            logger = Log.ForContext<TwitchFacade>();
        }

        public async Task RunBotAsync()
        {
            string channelName = options.ChannelName;

            await twitchIrcService.ConnectAsync();
            logger.Information("Connected to {0}", options.ChannelName);
            
            while (!ShouldStop)
            {
                string line = await twitchIrcService.GetMessageAsync();
                if (line == null) continue;

                logger.Debug(line);

                var watch = new Stopwatch();
                string[] split = line.Split(" ");
                if (line.StartsWith("PING"))
                {
                    logger.Debug("PING");
                    await twitchIrcService.SendPongAsync(split[1]);
                }
                else if (line.Contains("PRIVMSG"))
                {
                    var tokens = line.Split(":", StringSplitOptions.RemoveEmptyEntries);
                    var comandos = tokens.Last().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (!comandos.Any()) continue;

                    var commandName = comandos.First().Trim();
                    var commandArgs = comandos.Skip(1);

                    var command = commandFactory.Get(commandName);
                    if (command == null) continue;

                    watch.Restart();
                    var message = await command.ExecuteAsync(commandArgs?.ToArray());
                    watch.Stop();

                    logger.Information("Command {0} executed in {1} ms", commandName, watch.ElapsedMilliseconds);

                    await twitchIrcService.SendMessageAsync(channelName, message);
                }
            }
        }
    }
}

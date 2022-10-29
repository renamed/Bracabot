using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Serilog;

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
            
            while (!ShouldStop)
            {
                string line = await twitchIrcService.GetMessageAsync();
                if (line == null) continue;

                logger.Information(line);

                string[] split = line.Split(" ");
                if (line.StartsWith("PING"))
                {
                    logger.Information("PING");
                    await twitchIrcService.SendPongAsync(split[1]);
                }
                else if (line.Contains("PRIVMSG"))
                {
                    var tokens = line.Split(":", StringSplitOptions.RemoveEmptyEntries);
                    var comandos = tokens.Last().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (!comandos.Any()) continue;

                    var comando = comandos.First().Trim();
                    var parametros = comandos.Skip(1);

                    var command = commandFactory.Get(comando);
                    if (command == null) continue;

                    var message = await command.ExecuteAsync(parametros?.ToArray());
                    await twitchIrcService.SendMessageAsync(channelName, message);
                }
            }
        }
    }
}

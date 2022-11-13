using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Bracabot2.Domain.TwitchChat.Parsers;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;

namespace Bracabot2.Services
{
    public class TwitchFacade : IBotFacade
    {
        public volatile bool ShouldStop;

        private readonly ITwitchMessageHandler twitchMessageHandler;
        
        private readonly ILogger logger;
        private readonly SettingsOptions options;
        private readonly IIrcService twitchIrcService;

        public TwitchFacade(IOptions<SettingsOptions> options, IIrcService twitchIrcService, ITwitchMessageHandler twitchMessageHandler)
        {
            this.options = options.Value;
            this.twitchIrcService = twitchIrcService;

            logger = Log.ForContext<TwitchFacade>();
            this.twitchMessageHandler = twitchMessageHandler;
        }

        public async Task RunBotAsync()
        {
            await twitchIrcService.ConnectAsync();
            logger.Information("Connected to {0}", options.ChannelName);

            while (!ShouldStop)
            {
                string line = await twitchIrcService.GetMessageAsync();
                if (line is null) continue;

                logger.Debug(line);

                var parsed = TwitchMessageParser.Parse(line);
                if (parsed is null)
                    continue;

                var message = await twitchMessageHandler.Handle(parsed);
                await twitchIrcService.SendMessageAsync(message);

            }
        }
    }
}

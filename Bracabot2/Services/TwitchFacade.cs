using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;

namespace Bracabot2.Services
{
    public class TwitchFacade : IBotFacade
    {
        public volatile bool ShouldStop;

        private readonly CommandFactory commandFactory;

        public TwitchFacade(CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        public async Task RunBotAsync()
        {
            string channelName = Environment.GetEnvironmentVariable("CHANNEL_NAME");
            var twitchIrcService = new TwitchIrcService();

            await twitchIrcService.ConnectAsync();
            
            while (!ShouldStop)
            {
                string line = await twitchIrcService.GetMessageAsync();
                if (line == null) continue;

                Console.WriteLine(line);

                string[] split = line.Split(" ");
                if (line.StartsWith("PING"))
                {
                    Console.WriteLine("PING");
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

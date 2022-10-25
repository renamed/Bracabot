using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Bracabot2.Services;
using Microsoft.Extensions.DependencyInjection;


            Config.AddEnvironmentVariables();

IServiceCollection services = new ServiceCollection();
services.AddSingleton<IDotaService, DotaService>()
            .AddSingleton<CommandFactory>()
            .AddSingleton<IBotFacade, TwitchFacade>()
            .AddSingleton<IIrcService, TwitchIrcService>()
            .AddSingleton<ITwitchService, TwitchService>()
            .AddSingleton<IWebApiService, WebApiService>()
            .AddSingleton(sp => sp);

            await twitchIrcService.ConnectAsync();

var commandType = typeof(ICommand);
var allCommands = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => commandType.IsAssignableFrom(p) && !p.IsInterface);

foreach (var currentCommand in allCommands)
    services.AddSingleton(currentCommand);

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

var serviceProvider = services.BuildServiceProvider();

var botFacade = serviceProvider.GetService<IBotFacade>();

await botFacade.RunBotAsync();
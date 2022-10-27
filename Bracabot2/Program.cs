using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Bracabot2.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

Config.AddEnvironmentVariables();

IServiceCollection services = new ServiceCollection();
services.AddSingleton<IDotaService, DotaService>()
            .AddSingleton<CommandFactory>()
            .AddSingleton<IBotFacade, TwitchFacade>()
            .AddSingleton<IIrcService, TwitchIrcService>()
            .AddSingleton<ITwitchService, TwitchService>()
            .AddSingleton<IWebApiService, WebApiService>()
            .AddSingleton(sp => sp);

var commandType = typeof(ICommand);
var allCommands = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => commandType.IsAssignableFrom(p) && !p.IsInterface);

foreach (var currentCommand in allCommands)
    services.AddSingleton(currentCommand);

services.AddLogging(configure => configure.AddConsole())
    .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

var serviceProvider = services.BuildServiceProvider();

var botFacade = serviceProvider.GetService<IBotFacade>();

await botFacade.RunBotAsync();
using Bracabot2.Commands;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Bracabot2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

//var builder = new ConfigurationBuilder()
//    .SetBasePath(AppContext.BaseDirectory)
//    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

//services.Configure<Settings>(builder.Build().GetSection("Settings"));

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();



using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();

        IHostEnvironment env = hostingContext.HostingEnvironment;

        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

        IConfigurationRoot configurationRoot = configuration.Build();

        services.Configure<SettingsOptions>(configurationRoot.GetSection("Settings"));


    })
    .Build();


var serviceProvider = services.BuildServiceProvider();

var botFacade = serviceProvider.GetService<IBotFacade>();

await botFacade.RunBotAsync();
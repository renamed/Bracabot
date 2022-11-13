using AutoMapper;
using Bracabot2.Commands;
using Bracabot2.Crons;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Bracabot2.Repository;
using Bracabot2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Serilog;
using System.Reflection;

Config.AddEnvironmentVariables();

IServiceCollection services = new ServiceCollection();

var mapperConfig = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
IMapper mapper = mapperConfig.CreateMapper();
services.AddSingleton<IDotaService, DotaService>()
            .AddSingleton<CommandFactory>()
            .AddSingleton<IBotFacade, TwitchFacade>()
            .AddSingleton<IIrcService, TwitchIrcService>()
            .AddSingleton<ITwitchService, TwitchService>()
            .AddSingleton<IWebApiService, WebApiService>()
            .AddSingleton<IDotaRepository, DotaRepository>()
            .AddSingleton<IStreamOnlineVerifierJob, TwitchStreamStatusVerifierJob>()
            .AddSingleton<IRecentMatchesJob, DotaRecentMatchesJob>()
            .AddSingleton<ITwitchMessageHandler, TwitchMessageHandler>()
            .AddSingleton(mapper)
            .AddSingleton(sp => sp);
services.AddDbContext<DotaContext>();

var commandType = typeof(ICommand);
var allCommands = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => commandType.IsAssignableFrom(p) && !p.IsInterface);

foreach (var currentCommand in allCommands)
    services.AddSingleton(currentCommand);


services.AddLogging(configure =>
{
    configure.AddFilter("Microsoft", LogLevel.Warning)
           .AddConsole();
});

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(Path.Combine("logs", DateTime.UtcNow.Ticks + ".log"))        
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

services.AddHttpClient<IDotaService, DotaService>()
        .ConfigureHttpClient((serviceProvider, httpClient) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<SettingsOptions>>();
    httpClient.BaseAddress = new Uri(config.Value.Apis.Dota.BaseAddress);
})
        .SetHandlerLifetime(TimeSpan.FromMinutes(60));

services.AddHttpClient<TwitchService>(Consts.Clients.TWITCH_API_CLIENT)
        .ConfigureHttpClient((serviceProvider, httpClient) =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<SettingsOptions>>();
            httpClient.BaseAddress = new Uri(config.Value.Apis.Twitch.BaseAddress);
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(60));

services.AddHttpClient<TwitchService>(Consts.Clients.TWITCH_TOKEN_API_CLIENT)
        .ConfigureHttpClient((serviceProvider, httpClient) =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<SettingsOptions>>();
            httpClient.BaseAddress = new Uri(config.Value.Apis.Twitch.BaseAddressToken);
            httpClient.Timeout = TimeSpan.FromSeconds(Consts.Clients.TWITCH_API_TOKEN_TIMEOUT);
        });

services.AddMemoryCache();
services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

var serviceProvider = services.BuildServiceProvider();
var jobFactory = new JobFactory(serviceProvider);

IScheduler scheduler = await SchedulerBuilder.Create()
    .UseDefaultThreadPool(x => x.MaxConcurrency = 1)
    .BuildScheduler();
scheduler.JobFactory = jobFactory;

IJobDetail twitchStreamStatusVerifierJob = JobBuilder.Create<IStreamOnlineVerifierJob>()
                .Build();

ITrigger twitchStreamStatusVerifierJobTrigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

await scheduler.Start();
await scheduler.ScheduleJob(twitchStreamStatusVerifierJob, twitchStreamStatusVerifierJobTrigger);


var botFacade = serviceProvider.GetService<IBotFacade>();
await botFacade.RunBotAsync();
await scheduler.Shutdown();
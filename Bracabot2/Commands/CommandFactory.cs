using Microsoft.Extensions.DependencyInjection;

namespace Bracabot2.Commands
{
    public class CommandFactory
    {
        private readonly IDictionary<string, ICommand> commands;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
            {
                { "!comandos",  serviceProvider.GetService<CommandsCommand>() },
                { "!commands",  serviceProvider.GetService<CommandsCommand>() },
                { "!commandos", serviceProvider.GetService<CommandsCommand>() },
                { "!comands",   serviceProvider.GetService<CommandsCommand>() },

                { "!liberdade", serviceProvider.GetService<FreedomCommand>() },

                { "!aproveitamento", serviceProvider.GetService<PerformanceCommand>() },

                { "!recalibrar"  , serviceProvider.GetService<RecalibrationCommand>() },
                { "!recalibracao", serviceProvider.GetService<RecalibrationCommand>() },
                { "!recalibração", serviceProvider.GetService<RecalibrationCommand>() },
                { "!recal"       , serviceProvider.GetService<RecalibrationCommand>() },

                { "!placar", serviceProvider.GetService<ScoreCommand>() },
                { "!wl",     serviceProvider.GetService<ScoreCommand>() },
                { "!score",  serviceProvider.GetService<ScoreCommand>() },

                { "!heroi", serviceProvider.GetService<HeroCommand>() },
                { "!herói", serviceProvider.GetService<HeroCommand>() },
                { "!hero",  serviceProvider.GetService<HeroCommand>() },

                { "!medalha" ,  serviceProvider.GetService<MedalCommand>() },
                {"!medal",      serviceProvider.GetService<MedalCommand>()},
                {"!mmr",        serviceProvider.GetService<MedalCommand>()},

                {"!tai", serviceProvider.GetService<PingCommand>() },

                {"!histograma", serviceProvider.GetService<HistogramCommand>() }
        };
        }

        public ICommand? Get(string name)
        {
            _ = commands.TryGetValue(name, out var command);
            return command;
        }
    }
}

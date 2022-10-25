using System;
using System.Collections.Generic;

namespace Bracabot2.Commands
{
    public static class CommandFactory
    {
        private static IDictionary<string, ICommand> _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            { "!comandos", new CommandsCommand() },
            { "!commands", new CommandsCommand() },
            { "!commandos",new CommandsCommand() },
            { "!comands",  new CommandsCommand() },

            { "!liberdade",  new FreedomCommand() },

            { "!aproveitamento", new PerformanceCommand() },

            { "!recalibrar", new RecalibrationCommand() },
            { "!recalibracao", new RecalibrationCommand() },
            { "!recalibração", new RecalibrationCommand() },
            { "!recal", new RecalibrationCommand() },

            { "!placar", new ScoreCommand() },
            { "!wl", new ScoreCommand() },
            { "!score", new ScoreCommand() },

            { "!heroi", new HeroCommand() },
            { "!herói", new HeroCommand() },
            { "!hero", new HeroCommand() },

            { "!medalha" , new MedalCommand() },
            {"!medal", new MedalCommand()},
            {"!mmr", new MedalCommand()},

            {"!tai", new PingCommand() },

            {"!histograma", new HistogramCommand() }
        };

        public static ICommand Get(string name)
        {
            _ = _commands.TryGetValue(name, out var command);
            return command;
        }
    }
}

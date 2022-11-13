using AutoMapper;
using Bracabot2.Domain.Games.Dota2;
using Bracabot2.Domain.Interfaces;
using Bracabot2.Domain.Support;
using Microsoft.Extensions.Options;
using Quartz;
using Serilog;

namespace Bracabot2.Crons
{
    [DisallowConcurrentExecution]
    public class DotaRecentMatchesJob : IRecentMatchesJob
    {
        private readonly ILogger logger;
        private readonly IDotaRepository dotaRepository;
        private readonly IDotaService dotaService;
        private readonly IMapper mapper;
        private readonly SettingsOptions config;

        public DotaRecentMatchesJob(IDotaRepository dotaRepository, IDotaService dotaService, IMapper mapper, IOptions<SettingsOptions> config)
        {
            this.logger = Log.ForContext<DotaRecentMatchesJob>();
            this.dotaRepository = dotaRepository;
            this.dotaService = dotaService;
            this.mapper = mapper;
            this.config = config.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            logger.Debug("Initializing {0} cron job", nameof(DotaRecentMatchesJob));
            try
            {
                var dotaId = config.DotaId;

                var lastMatch = await dotaRepository.GetLastMatchAsync();
                var daysToSearch = lastMatch != null
                    ? Math.Max(1, (DateTime.UtcNow - lastMatch.EndTime).TotalDays + 1)
                    : int.MaxValue;
                var apiMatches = await dotaService.GetMatchesAsync(dotaId, (int)daysToSearch);

                logger.Debug("Number of matches found from the dota 2 API: {0}", apiMatches.Count());

                var matchesCount = await dotaRepository.AddIfNotExistsAsync(apiMatches.Select(x => mapper.Map<Match>(x)));

                logger.Debug("Number of matches inserted into database: {0}", matchesCount);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to update de dota 2 games match history {0}", ex.Message);
            }
            logger.Debug("Finishing {0} cron job", nameof(DotaRecentMatchesJob));
        }
    }
}

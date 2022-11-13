using Bracabot2.Domain.Interfaces;
using Quartz;
using Serilog;

namespace Bracabot2.Crons
{    
    public class TwitchStreamStatusVerifierJob : IStreamOnlineVerifierJob
    {
        private readonly ITwitchService twitchService;
        private readonly ILogger logger;

        public TwitchStreamStatusVerifierJob(ITwitchService twitchService)
        {
            this.twitchService = twitchService;
            this.logger = Log.ForContext<TwitchStreamStatusVerifierJob>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            logger.Debug("Initializing {0} cron job", nameof(TwitchStreamStatusVerifierJob));

            var rescheduleTime = TimeSpan.FromMinutes(10);
            var streamInfo = await twitchService.GetStreamInfo();
            if (streamInfo == null)
            {
                logger.Debug("Stream is not online");
                rescheduleTime = TimeSpan.FromHours(1);
            }
            else if (streamInfo.IsDota2Game)
            {
                logger.Debug("Stream is online and game is correct");
                rescheduleTime = TimeSpan.FromMinutes(1);
                await LaunchDotaRecentMatchesJob(context);                
            }

            logger.Debug("Scheduling {0} to {1}", nameof(TwitchStreamStatusVerifierJob), rescheduleTime);
            await RescheduleThisJob(context, rescheduleTime);
            logger.Debug("Finishing {0} cron job", nameof(TwitchStreamStatusVerifierJob));
        }

        private async Task LaunchDotaRecentMatchesJob(IJobExecutionContext context)
        {
            var scheduler = context.Scheduler;

            IJobDetail jobDetail = JobBuilder.Create<IRecentMatchesJob>()
                .Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                            .StartNow()
                            .Build();

            await scheduler.ScheduleJob(jobDetail, jobTrigger);
        }

        private async Task RescheduleThisJob(IJobExecutionContext context, TimeSpan schedule)
        {
            var scheduler = context.Scheduler;
            var triggerKey = context.Trigger.Key;

            ITrigger newTrigger = TriggerBuilder.Create()
                .StartAt(DateTimeOffset.UtcNow.Add(schedule))
                .Build();
                        
            await scheduler.RescheduleJob(triggerKey, newTrigger);
        }
    }
}

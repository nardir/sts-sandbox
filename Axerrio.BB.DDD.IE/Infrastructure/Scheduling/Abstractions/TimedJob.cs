using EnsureThat;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.Scheduling.Abstractions
{
    [DisallowConcurrentExecution]
    public abstract class TimedJob : IJob
    {
        protected readonly ILogger _logger;

        public TimedJob(ILogger logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }
        public abstract Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));

        public async Task Execute(IJobExecutionContext context)
        {
            await context.Scheduler.PauseAll();

            try
            {
                await ExecuteAsync(context.CancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"Unhandled critical exception {exception.GetType().Name} with message ${exception.Message} for timed job");
            }
            finally
            {
                await context.Scheduler.ResumeAll();
            }
        }
    }
}

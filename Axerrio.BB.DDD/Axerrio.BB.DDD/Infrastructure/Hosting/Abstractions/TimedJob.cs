using EnsureThat;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.Hosting.Abstractions
{
    [DisallowConcurrentExecution]
    public abstract class TimedJob : IJob
    {
        private readonly ILogger _logger;

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
            catch (Exception ex)
            {

            }
            finally
            {
                await context.Scheduler.ResumeAll();
            }
        }
    }
}

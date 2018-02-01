using Axerrio.BB.DDD.Infrastructure.Hosting.HostedServices.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Scheduling.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.Hosting.HostedServices
{
    public class TimedHostedService<TJob, TTriggerFactory> : BackgroundService
        where TJob : TimedJob
        where TTriggerFactory : ITriggerFactory
    {
        private readonly ILogger<TimedHostedService<TJob, TTriggerFactory>> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private readonly IJobDetail _jobDetail;
        private readonly ITrigger _trigger;
        private readonly IJobFactory _jobFactory;

        public TimedHostedService(ILogger<TimedHostedService<TJob, TTriggerFactory>> logger, IJobFactory jobFactory, TTriggerFactory triggerFactory)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            //http://www.quartz-scheduler.org/documentation/quartz-2.x/configuration/ConfigMain.html
            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

            _schedulerFactory = new StdSchedulerFactory(props);


            //Build trigger
            _trigger = EnsureArg.IsNotNull(triggerFactory, nameof(triggerFactory)).Create();

            _jobFactory = EnsureArg.IsNotNull(jobFactory, nameof(jobFactory));

            //Build job
            //IDictionary<string, object> map = new Dictionary<string, object>();
            //map.Add("Logger", _logger);

            //var jobDataMap = new JobDataMap(map);

            //Type jobType = typeof(TJob);

            _jobDetail = JobBuilder.Create<TJob>()
                                    .WithIdentity(typeof(TJob).Name, _trigger.Key.Group)
                                    //.UsingJobData(jobDataMap)
                                    .Build();


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.Register(() => _logger.LogDebug($"Timed hosted service for job {_jobDetail.Key.Name} and trigger {_trigger.Key.Name} is stopping"));

                //throw new InvalidOperationException();

                _scheduler = await _schedulerFactory.GetScheduler(stoppingToken);

                _scheduler.JobFactory = _jobFactory;

                await _scheduler.ScheduleJob(_jobDetail, _trigger, stoppingToken);

                await _scheduler.Start(stoppingToken);

                _logger.LogDebug($"Timed hosted service started for job {_jobDetail.Key.Name} and trigger {_trigger.Key.Name}");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, $"Timed hosted service could not start for job {_jobDetail.Key.Name} and trigger {_trigger.Key.Name}");

                await ShutDownScheduler(stoppingToken);
            }
        }

        private async Task ShutDownScheduler(CancellationToken cancellationToken)
        {
            if (_scheduler?.IsStarted == true)
            {
                await _scheduler.Shutdown(cancellationToken);

                _logger.LogDebug($"Timed hosted service scheduler stopped for job {_jobDetail.Key.Name} and trigger {_trigger.Key.Name}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await ShutDownScheduler(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}

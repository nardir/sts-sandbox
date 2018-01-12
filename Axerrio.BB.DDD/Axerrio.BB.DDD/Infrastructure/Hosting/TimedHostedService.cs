using EnsureThat;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.Hosting
{
    public class TimedHostedService<TJob> : BackgroundService
        where TJob : IJob
    {
        private readonly ILogger<TimedHostedService<TJob>> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private readonly IJobDetail _job;
        private readonly ITrigger _trigger;
        private readonly IJobFactory _jobFactory;

        public TimedHostedService(ILogger<TimedHostedService<TJob>> logger, IJobFactory jobFactory)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

            _schedulerFactory = new StdSchedulerFactory(props);

            _jobFactory = EnsureArg.IsNotNull(jobFactory, nameof(jobFactory));

            //Build job
            //IDictionary<string, object> map = new Dictionary<string, object>();
            //map.Add("Logger", _logger);

            //var jobDataMap = new JobDataMap(map);

            _job = JobBuilder.Create<TJob>()
                                    .WithIdentity(nameof(TJob))
                                    //.UsingJobData(jobDataMap)
                                    .Build();

            //Build trigger
            _trigger = TriggerBuilder.Create()
                                    .WithIdentity("trigger1")
                                    .StartNow()
                                    .WithSimpleSchedule(x => x
                                        .WithIntervalInSeconds(10)
                                        .RepeatForever())
                                    .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
            _scheduler = await _schedulerFactory.GetScheduler(stoppingToken);

            _scheduler.JobFactory = _jobFactory;

            await _scheduler.ScheduleJob(_job, _trigger, stoppingToken);

            await _scheduler.Start(stoppingToken);

            //await Task.Delay(Timeout.Infinite, stoppingToken);

            //await scheduler.Shutdown();

            //_logger.LogInformation("TimedHostedService is stopping");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown();

            await base.StopAsync(cancellationToken);
        }
    }

    public class TestJob : IJob
    {
        //public ILogger Logger { get; set; }
        private readonly ILogger<TestJob> _logger;

        public TestJob(ILogger<TestJob> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(_logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await context.Scheduler.PauseAll();

            //Logger.LogInformation($"{context.JobDetail.Key}");
            _logger.LogInformation($"{context.JobDetail.Key}");

            await context.Scheduler.ResumeAll();

            //return Task.CompletedTask;
        }
    }

    public class JobFactory : IJobFactory
    {
        //https://stackoverflow.com/questions/42157775/net-core-quartz-dependency-injection

        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;

            disposable?.Dispose();
        }
    }
}

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
    public class TimedHostedService<TJob, TTriggerFactory> : BackgroundService
        where TJob : IJob
        where TTriggerFactory : ITriggerFactory
    {
        private readonly ILogger<TimedHostedService<TJob, TTriggerFactory>> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private readonly IJobDetail _job;
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
            _trigger = EnsureArg.IsNotNull(triggerFactory, nameof(triggerFactory)).Create();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler(stoppingToken);

            _scheduler.JobFactory = _jobFactory;

            await _scheduler.ScheduleJob(_job, _trigger, stoppingToken);

            await _scheduler.Start(stoppingToken);
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

            //context.CancellationToken

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

    public interface ITriggerFactory
    {
        ITrigger Create();
    }

    public class TestTriggerFactory : ITriggerFactory
    {
        private readonly int _intervalInSeconds;
        private readonly int _intervalInMilliseconds;

        public TestTriggerFactory()
        {
            _intervalInSeconds = 5; //Fetch from options supplied by DI
            _intervalInMilliseconds = 5000;
        }

        public ITrigger Create()
        {
            return TriggerBuilder.Create()
                        .WithIdentity("TestTrigger")
                        .StartNow()
                        .WithSimpleSchedule(sb => sb
                            //.WithIntervalInSeconds(_intervalInSeconds)
                            .WithInterval(TimeSpan.FromMilliseconds(_intervalInMilliseconds))
                            .RepeatForever())
                        .Build();
        }
    }
}

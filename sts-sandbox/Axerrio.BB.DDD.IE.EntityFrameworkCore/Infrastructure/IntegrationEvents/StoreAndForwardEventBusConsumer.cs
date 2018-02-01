using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Scheduling.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class StoreAndForwardEventBusConsumer<TConsumerTriggerFactory, TConsumerJob> : IEventBusConsumer
        where TConsumerTriggerFactory : ITriggerFactory
        where TConsumerJob : TimedJob
    {
        private readonly ILogger<StoreAndForwardEventBusConsumer<TConsumerTriggerFactory, TConsumerJob>> _logger;
        //private readonly ITriggerFactory _consumerTriggerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobDetail _jobDetail;
        private readonly ITrigger _consumerTrigger;

        private IScheduler _scheduler;

        public StoreAndForwardEventBusConsumer(TConsumerTriggerFactory consumerTriggerFactory
            , IJobFactory jobFactory
            , ILogger<StoreAndForwardEventBusConsumer<TConsumerTriggerFactory, TConsumerJob>> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            //_consumerTriggerFactory = EnsureArg.IsNotNull(consumerTriggerFactory, nameof(consumerTriggerFactory));
            _jobFactory = EnsureArg.IsNotNull(jobFactory, nameof(jobFactory));
            _consumerTrigger = EnsureArg.IsNotNull(consumerTriggerFactory, nameof(consumerTriggerFactory)).Create();

            //http://www.quartz-scheduler.org/documentation/quartz-2.x/configuration/ConfigMain.html
            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

            _schedulerFactory = new StdSchedulerFactory(props);

            _jobDetail = JobBuilder.Create<TConsumerJob>()
                        .WithIdentity(typeof(TConsumerJob).Name, _consumerTrigger.Key.Group)
                        .Build();
        }

        #region IEventBusConsumer

        public async Task StartConsumerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await ShutDownScheduler(cancellationToken);

            _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            _scheduler.JobFactory = _jobFactory;

            await _scheduler.ScheduleJob(_jobDetail, _consumerTrigger, cancellationToken);

            await _scheduler.Start(cancellationToken);
        }

        public Task StopConsumerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return ShutDownScheduler(cancellationToken);
        }

        #endregion

        private async Task ShutDownScheduler(CancellationToken cancellationToken)
        {
            if (_scheduler?.IsStarted == true)
            {
                await _scheduler.Shutdown(cancellationToken);
            }
        }
    }
}

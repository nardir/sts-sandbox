using Axerrio.BB.DDD.Infrastructure.Scheduling.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class StoreAndForwardEventBusConsumerTriggerFactory: ITriggerFactory
    {
        private readonly StoreAndForwardEventBusConsumerOptions _consumerOptions;

        public StoreAndForwardEventBusConsumerTriggerFactory(IOptions<StoreAndForwardEventBusConsumerOptions> consumerOptionsAccessor)
        {
            _consumerOptions = EnsureArg.IsNotNull(consumerOptionsAccessor, nameof(consumerOptionsAccessor)).Value;
        }

        public ITrigger Create()
        {
            return TriggerBuilder.Create()
            .WithIdentity("StoreAndForwardEventBusConsumerTrigger", "StoreAndForwardEventBusConsumer")
            .StartNow()
            .WithSimpleSchedule(sb => sb
                .WithInterval(TimeSpan.FromMilliseconds(_consumerOptions.TriggerIntervalInMilliseconds))
                .RepeatForever())
            .Build();

        }

    }
}

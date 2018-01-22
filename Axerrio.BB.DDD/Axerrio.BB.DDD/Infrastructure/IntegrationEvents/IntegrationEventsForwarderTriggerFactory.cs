using Axerrio.BB.DDD.Infrastructure.Hosting.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{ 
    public class IntegrationEventsForwarderTriggerFactory : ITriggerFactory
    {
        private readonly IntegrationEventsForwarderTriggerOptions _integrationEventsForwarderTriggerOptions;

        public IntegrationEventsForwarderTriggerFactory(IOptions<IntegrationEventsForwarderTriggerOptions> integrationEventsForwarderTriggerOptionsAccessor)
        {
            _integrationEventsForwarderTriggerOptions = EnsureArg.IsNotNull(integrationEventsForwarderTriggerOptionsAccessor, nameof(integrationEventsForwarderTriggerOptionsAccessor)).Value;
        }

        public ITrigger Create()
        {
            return TriggerBuilder.Create()
            .WithIdentity("IntegrationEventsForwarderTrigger", "IntegrationEventsForwarder")
            .StartNow()
            .WithSimpleSchedule(sb => sb
                .WithInterval(TimeSpan.FromMilliseconds(_integrationEventsForwarderTriggerOptions.IntervalInMilliseconds))
                .RepeatForever())
            .Build();

        }
    }
}

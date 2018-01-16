using Axerrio.BB.DDD.Infrastructure.Hosting.Abstractions;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{ 
    public class IntegrationEventsForwarderTriggerFactory : ITriggerFactory
    {
        private readonly int _intervalInMilliseconds;

        public IntegrationEventsForwarderTriggerFactory()
        {
            _intervalInMilliseconds = 200;
        }

        public ITrigger Create()
        {
            return TriggerBuilder.Create()
            .WithIdentity("IntegrationEventsForwarderTrigger")
            .StartNow()
            .WithSimpleSchedule(sb => sb
                .WithInterval(TimeSpan.FromMilliseconds(_intervalInMilliseconds))
                .RepeatForever())
            .Build();

        }
    }
}

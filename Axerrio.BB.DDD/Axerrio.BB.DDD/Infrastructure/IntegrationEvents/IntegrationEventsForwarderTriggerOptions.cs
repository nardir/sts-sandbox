using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsForwarderTriggerOptions
    {
        public int IntervalInMilliseconds { get; set; } = 200;
    }
}

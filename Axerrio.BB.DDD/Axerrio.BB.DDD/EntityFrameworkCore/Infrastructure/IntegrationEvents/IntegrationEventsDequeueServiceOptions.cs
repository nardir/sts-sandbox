using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsDequeueServiceOptions
    {
        public IntegrationEventsDequeueServiceOptions()
        {
            MaxEventsToDequeue = 10;
            MaxPublishAttempts = 3;
        }

        public int MaxEventsToDequeue { get; set; }
        public int MaxPublishAttempts { get; set; }
    }
}
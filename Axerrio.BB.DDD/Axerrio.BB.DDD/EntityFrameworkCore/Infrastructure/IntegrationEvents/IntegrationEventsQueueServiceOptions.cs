using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsQueueServiceOptions
    {
        public IntegrationEventsQueueServiceOptions()
        {
            MaxEventsToDequeue = 10;
            MaxPublishAttempts = 3;
        }

        public int MaxEventsToDequeue { get; set; }
        public int MaxPublishAttempts { get; set; }
    }
}
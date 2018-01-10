using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public enum IntegrationEventsQueueItemState
    {
        NotPublished = 0,
        Publishing = 1,
        Published = 2,
        PublishedFailed = 99
    }
}
using Axerrio.BB.DDD.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public enum IntegrationEventsQueueItemState
    {
        NotPublished = 0,
        Publishing = 1,
        Published = 2,
        PublishedFailed = 99
    }

    //public class IntegrationEventsQueueItemState: Enumeration<IntegrationEventsQueueItemState>
    //{
    //    public static IntegrationEventsQueueItemState NotPublished = new IntegrationEventsQueueItemState(0, nameof(NotPublished));
    //    public static IntegrationEventsQueueItemState Publishing = new IntegrationEventsQueueItemState(1, nameof(Publishing));
    //    public static IntegrationEventsQueueItemState Published = new IntegrationEventsQueueItemState(2, nameof(Published));
    //    public static IntegrationEventsQueueItemState PublishedFailed = new IntegrationEventsQueueItemState(99, nameof(PublishedFailed));

    //    static IntegrationEventsQueueItemState()
    //    {
    //        _items.AddRange(new[] { NotPublished, Publishing, Published, PublishedFailed });
    //    }

    //    protected IntegrationEventsQueueItemState() {}

    //    protected IntegrationEventsQueueItemState(int id, string name)
    //        : base (id, name)
    //    {               
    //    }
    //}
}

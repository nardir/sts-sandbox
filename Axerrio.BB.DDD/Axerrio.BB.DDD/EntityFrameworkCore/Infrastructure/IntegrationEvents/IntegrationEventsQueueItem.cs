using Axerrio.BB.DDD.Application.IntegrationEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsQueueItem
    {
        private IntegrationEventsQueueItem() { }

        public IntegrationEventsQueueItem(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = IntegrationEventsQueueItemState.NotPublished;
            TimesSent = 0;
        }

        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public IntegrationEventsQueueItemState State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
    }
}

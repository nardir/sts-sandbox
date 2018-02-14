using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class IntegrationEventsQueueItem
    {
        private IntegrationEventsQueueItem() { }

        public IntegrationEventsQueueItem(IntegrationEvent @event)
        {
            State = IntegrationEventsQueueItemState.NotPublished;

            PublishAttempts = 0;
            PublishBatchId = null;

            LatestDequeuedTimestamp = null;
            PublishedTimestamp = null;
            PublishedFailedTimestamp = null;
            RequeuedTimestamp = null;

            EventId = @event.Id;
            EventCreationTimestamp = @event.CreationTimestamp;
            EventTypeName = @event.GetType().FullName;
            EventContent = JsonConvert.SerializeObject(@event);
        }

        [JsonIgnore]
        public IntegrationEvent IntegrationEvent {
            get
            {
                Type type = Type.GetType(EventTypeName);

                var @event = JsonConvert.DeserializeObject(EventContent, type);

                return (IntegrationEvent) @event;
            }
        }

        //Queue properties
        public int EventQueueItemId { get; set; }
        public IntegrationEventsQueueItemState State { get; set; }
        public int PublishAttempts { get; set; }
        public Guid? PublishBatchId { get; set; }

        public DateTime EnqueuedTimestamp { get; set; }
        public DateTime? LatestDequeuedTimestamp { get; set; }
        public DateTime? PublishedTimestamp { get; set; }
        public DateTime? PublishedFailedTimestamp { get; set; }
        public DateTime? RequeuedTimestamp { get; set; }

        //Event properties
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public DateTime EventCreationTimestamp { get; private set; }
        public string EventContent { get; private set; }
    }
}
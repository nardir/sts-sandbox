using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsQueueItem
    {
        private IntegrationEventsQueueItem() { }

        public static IntegrationEventsQueueItem Create(IntegrationEvent @event)
        {
            var integrationEventsQueueItem = new IntegrationEventsQueueItem(@event.Id
                , @event.CreationTimestamp
                , @event.GetEventName()
                , JsonConvert.SerializeObject(@event));

            integrationEventsQueueItem.EventTypeName = @event.GetType().FullName;

            return integrationEventsQueueItem;
        }

        public static IntegrationEventsQueueItem Create(string eventName, string eventMessage)
        {
            dynamic @event = JObject.Parse(eventMessage);

            return new IntegrationEventsQueueItem(@event.Id
                , @event.CreationTimestamp
                , eventName
                , eventMessage);
        }

        //public IntegrationEventsQueueItem(IntegrationEvent @event)
        //{
        //    State = IntegrationEventsQueueItemState.NotPublished;

        //    PublishAttempts = 0;
        //    PublishBatchId = null;

        //    LatestDequeuedTimestamp = null;
        //    PublishedTimestamp = null;
        //    PublishedFailedTimestamp = null;
        //    RequeuedTimestamp = null;

        //    EventId = @event.Id;
        //    EventCreationTimestamp = @event.CreationTimestamp;

        //    //EventName = IntegrationEvent.GetEventName(@event.GetType());

        //    //EventTypeName = @event.GetType().FullName;
        //    //EventTypeName = @event.GetType().AssemblyQualifiedName;
        //    //EventTypeName = IntegrationEvent.GetEventName(@event.GetType());
        //    EventTypeName = @event.GetEventName();

        //    EventContent = JsonConvert.SerializeObject(@event);
        //}

        public IntegrationEventsQueueItem(Guid eventId
            , DateTime eventCreationTimestamp
            , string eventName
            , string eventContent)
        {
            State = IntegrationEventsQueueItemState.NotPublished;

            PublishAttempts = 0;
            PublishBatchId = null;

            LatestDequeuedTimestamp = null;
            PublishedTimestamp = null;
            PublishedFailedTimestamp = null;
            RequeuedTimestamp = null;

            EventId = eventId;
            EventCreationTimestamp = eventCreationTimestamp;

            EventName = eventName;
            EventTypeName = string.Empty;

            EventContent = eventContent;
        }

        //[JsonIgnore]
        //public IntegrationEvent IntegrationEvent
        //{
        //    get
        //    {
        //        Type type = Type.GetType(EventTypeName);

        //        var @event = JsonConvert.DeserializeObject(EventContent, type);

        //        return (IntegrationEvent)@event;
        //    }
        //}

        ////https://blogs.msdn.microsoft.com/mazhou/2017/05/26/c-7-series-part-1-value-tuples/
        //public (string eventName, dynamic @event) GetDynamicIntegrationEvent()
        //{
        //    dynamic @event = JObject.Parse(EventContent);

        //    return (EventName, @event);
        //}

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
        public string EventName { get; private set; }
        public string EventTypeName { get; private set; }
        public DateTime EventCreationTimestamp { get; private set; }
        public string EventContent { get; private set; }
    }
}

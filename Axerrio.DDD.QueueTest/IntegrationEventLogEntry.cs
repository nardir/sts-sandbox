using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }
        public IntegrationEventLogEntry(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;

            //Retries ??
        }

        public IntegrationEventLogEntry(Guid eventId, string eventTypeName, string content)
        {
            EventId = eventId;
            CreationTime = DateTime.UtcNow;
            EventTypeName = eventTypeName;
            Content = content;
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }

        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }

        //Id int EF Shadow properties
    }
}

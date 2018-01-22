using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    //Domain
    public class StoredIntegrationEvent<TDomainEvent>: Entity<int, TDomainEvent>, IAggregateRoot
    {
        protected StoredIntegrationEvent() {}

        public StoredIntegrationEvent(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;

            //Retries ??
        }

        [JsonProperty]
        public Guid EventId { get; private set; }
        [JsonProperty]
        public string EventTypeName { get; private set; }
        [JsonProperty]
        public EventStateEnum State { get; set; }
        [JsonProperty]
        public int TimesSent { get; set; }
        [JsonProperty]
        public DateTime CreationTime { get; private set; }
        [JsonProperty]
        public string Content { get; private set; }
    }
}

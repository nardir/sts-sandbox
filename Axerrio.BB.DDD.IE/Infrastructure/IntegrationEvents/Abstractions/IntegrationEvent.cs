using Axerrio.BB.DDD.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public abstract class IntegrationEvent: ValueObject<IntegrationEvent>
    {
        public static string GetEventName<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        {
            return typeof(TIntegrationEvent).Name;
        }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationTimestamp = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreationTimestamp { get; }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Id;
            //yield return CreationTimestamp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {
        public DomainEventHandler()
        {
        }

        public abstract Task Handle(TDomainEvent notification, CancellationToken cancellationToken = default(CancellationToken));
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Axerrio.BuildingBlocks
{
    public interface IDomainEventsEntity
    {
        ReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}

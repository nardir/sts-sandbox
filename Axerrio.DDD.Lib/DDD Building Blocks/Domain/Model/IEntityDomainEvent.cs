using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Axerrio.BuildingBlocks
{
    public interface IEntityDomainEvents
    {
        ReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}

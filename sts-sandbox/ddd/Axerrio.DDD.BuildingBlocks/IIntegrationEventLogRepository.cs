using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public interface IIntegrationEventLogRepository: IRepository<IntegrationEvent>
    {
        void Add(IntegrationEvent @event);
    }
}

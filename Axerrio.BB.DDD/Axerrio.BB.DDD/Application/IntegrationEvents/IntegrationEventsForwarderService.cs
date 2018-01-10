using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class IntegrationEventsForwarderService : IIntegrationEventsForwarderService
    {
        public IntegrationEventsForwarderService(IEventBusPublishOnly eventBus)
        {

        }

        public Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //Dequeue Store
            //Publish eventbus
            //MarkAsPublished store

            //Exception:
            //If max retries bereikt dan MarkAsPublishedFailed
            //else MarkAsNotPublished

            throw new NotImplementedException();
        }
    }
}

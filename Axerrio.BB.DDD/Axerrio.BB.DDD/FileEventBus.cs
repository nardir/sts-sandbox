using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class FileEventBus : IEventBusPublishOnly
    {
        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = JsonConvert.SerializeObject(@event);

            return File.AppendAllTextAsync("eventbus.txt", message);
        }
    }
}

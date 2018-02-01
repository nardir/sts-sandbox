using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class StoreAndForwardEventBusConsumerOptions
    {
        public int TriggerIntervalInMilliseconds { get; set; }
    }
}

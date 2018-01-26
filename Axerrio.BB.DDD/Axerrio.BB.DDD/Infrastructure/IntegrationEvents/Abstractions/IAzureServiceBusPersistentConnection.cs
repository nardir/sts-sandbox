using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions
{
    public interface IAzureServiceBusPersistentConnection
    {
        ITopicClient CreateTopicClient();
        ISubscriptionClient CreateSubscriptionClient(); //SubscriptionClient
    }
}
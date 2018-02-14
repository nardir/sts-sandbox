using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class RabbitMQEventBusOptions
    {
        public int ConnectRetryAttempts { get; set; }
        public string Exchange { get; set; }
        public string QueueName { get; set; }
        public int PublishRetryAttempts { get; set; }
        public string ConnectionString { get; set; }
    }
}
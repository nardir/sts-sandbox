using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class EventBusOptions
    {
        //RabbitMQ: "amqp://user:pass@hostName:port/vhost"
        //Azure: "Endpoint=sb://sts-sandbox.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=okAS5ZUVaWkRQaoF8u6e8m7ulHZ+cZgu7nB0suhVh+M="
        public string ConnectionString { get; set; }
        public int ConnectRetryAttempts { get; set; }

        public string BrokerName { get; set; } //RabbitMQ : Exchange, Azure ServiceBus: Topic
        public string SubscriptionName { get; set; } //RabbitMQ : QueueName, Azure ServiceBus: SubscriptionName

        public int PublishRetryAttempts { get; set; }
    }
}

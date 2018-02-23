using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class StoreAndForwardEventBusDatabaseOptions
    {
        public StoreAndForwardEventBusDatabaseOptions()
        {
            Schema = "integrationevents";
            TableName = "EventQueueItem";
            MaxEventsToDequeue = 10;
            RetryAttempts = 3;
            RequeuePendingEventsPeriodInMinutes = 15;
            MaxPublishAttempts = 10;
        }

        public string ConnectionString { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public int MaxEventsToDequeue { get; set; }
        public int RetryAttempts { get; set; }
        public int RequeuePendingEventsPeriodInMinutes { get; set; }
        public int MaxPublishAttempts { get; set; }
    }
}

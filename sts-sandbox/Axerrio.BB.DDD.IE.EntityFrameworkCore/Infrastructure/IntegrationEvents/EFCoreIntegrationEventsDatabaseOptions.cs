using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreIntegrationEventsDatabaseOptions
    {
        public EFCoreIntegrationEventsDatabaseOptions()
        {
            Schema = "integrationevents";
            TableName = "EventQueueItem";
        }

        //public string ConnectionString { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
    }
}

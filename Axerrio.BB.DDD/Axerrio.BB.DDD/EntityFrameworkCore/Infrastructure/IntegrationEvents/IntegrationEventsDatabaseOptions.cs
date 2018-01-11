using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsDatabaseOptions
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
    }
}
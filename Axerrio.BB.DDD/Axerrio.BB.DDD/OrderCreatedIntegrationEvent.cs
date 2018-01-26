using Axerrio.BB.DDD.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class OrderCreatedIntegrationEvent: IntegrationEvent
    {
        public string OrderNumber { get; set; }
        public string CustomerCode { get; set; }
    }
}

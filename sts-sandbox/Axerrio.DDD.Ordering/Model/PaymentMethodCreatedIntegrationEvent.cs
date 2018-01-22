using Axerrio.DDD.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Model
{
    public class PaymentMethodCreatedIntegrationEvent: IntegrationEvent
    {
        public PaymentMethod PaymentMethod { get; set; }
    }
}

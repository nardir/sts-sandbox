using Axerrio.BB.DDD.Application.IntegrationEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class PaymentMethodCreatedIntegrationEvent : IntegrationEvent
    {
        private PaymentMethodCreatedIntegrationEvent()
        {

        }

        public PaymentMethodCreatedIntegrationEvent(PaymentMethod paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }

        //[JsonProperty]
        public PaymentMethod PaymentMethod { get; private set; }
    }
}

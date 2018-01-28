using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.IntegrationEvents
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public string OrderNumber { get; set; }
        public string CustomerCode { get; set; }

        //protected override IEnumerable<object> GetMemberValues()
        //{
        //    yield return base.GetMemberValues();

        //    yield return OrderNumber;
        //    yield return CustomerCode;
        //}
    }
}

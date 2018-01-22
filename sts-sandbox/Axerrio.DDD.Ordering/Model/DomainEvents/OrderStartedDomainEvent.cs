using Axerrio.DDD.BuildingBlocks;
using EnsureThat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Model.DomainEvents
{
    public class OrderStartedDomainEvent : ValueObject<OrderStartedDomainEvent>, INotification
    {
        public Order Order { get; private set; }

        public OrderStartedDomainEvent(Order order)
        {
            order = EnsureArg.IsNotNull(order, nameof(order));
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Order;
        }
    }
}

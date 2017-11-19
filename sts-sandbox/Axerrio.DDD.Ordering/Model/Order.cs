using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Model
{
    public class Order
    {
        public string BuyerName { get; private set; }
        protected int OrderStatusId { get; set; }
        public OrderStatus OrderStatus => OrderStatus.Parse(OrderStatusId);

        protected Order() { }

        public Order(string buyerName, OrderStatus orderStatus)
        {
            BuyerName = buyerName;
            OrderStatusId = orderStatus.Id;
        }
    }
}

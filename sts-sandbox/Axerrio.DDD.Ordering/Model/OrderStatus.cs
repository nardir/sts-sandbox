using Axerrio.DDD.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Model
{
    public class OrderStatus : Enumeration<OrderStatus>
    {
        public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new OrderStatus(4, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled).ToLowerInvariant());

        static OrderStatus()
        {
            Items.AddRange(new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Shipped, Cancelled });
        }

        protected OrderStatus() { }

        protected OrderStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}

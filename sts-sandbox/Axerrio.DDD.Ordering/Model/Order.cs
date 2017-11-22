using Axerrio.DDD.BuildingBlocks;
using Axerrio.DDD.Ordering.Model.DomainEvents;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Model
{
    //public class Order: Entity<int, INotification>
    public class Order : IntEntity
    {
        public override int Identity
        {
            get => base.Identity;

            protected set
            {
                if (value <= 0)
                    throw new ArgumentException();

                base.Identity = value;
            }
        }

        [JsonProperty]
        public string BuyerName { get; private set; }

        [JsonProperty]
        protected int OrderStatusId { get; set; }

        [JsonIgnore]
        public OrderStatus OrderStatus => OrderStatus.Parse(OrderStatusId);
        //public OrderStatus OrderStatus
        //{
        //    get => OrderStatus.Parse(OrderStatusId);

        //    //get
        //    //{
        //    //    if (!OrderStatus.TryParse(OrderStatusId, out OrderStatus orderStatus))
        //    //        return null;

        //    //    return orderStatus;
        //    //}

        //    //protected set
        //    //{
        //    //    if (value == null)
        //    //        throw new ArgumentNullException(nameof(OrderStatus));

        //    //    OrderStatusId = value.Id;
        //    //}
        //}

        protected Order() { }

        public Order(string buyerName, OrderStatus orderStatus)
            : this()
        {
            BuyerName = buyerName;
            OrderStatusId = orderStatus.Id;

            AddDomainEvent(new OrderStartedDomainEvent(this));
        }

        public Order(string buyerName, OrderStatus orderStatus, int identity)
            : this(buyerName, orderStatus)
        {
            //Test ctor, to be able to set identity
            Identity = identity;
        }
    }
}

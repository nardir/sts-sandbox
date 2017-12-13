using Axerrio.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate
{
    public class MenuStatus : Enumeration<MenuStatus>
    {
        public static MenuStatus Created = new MenuStatus(1, nameof(Created));   
        public static MenuStatus PickedUp = new MenuStatus(2, nameof(PickedUp));
//        public static MenuStatus Submitted = new MenuStatus(3, nameof(Submitted));
        public static MenuStatus AwaitingApproval = new MenuStatus(3, nameof(AwaitingApproval));
        public static MenuStatus ChangesRequested = new MenuStatus(4, nameof(ChangesRequested));
        public static MenuStatus Approved = new MenuStatus(5, nameof(Approved));           
        public static MenuStatus Cancelled = new MenuStatus(6, nameof(Cancelled));
        public static MenuStatus Send = new MenuStatus(7, nameof(Send));
             
        static MenuStatus()
        {
            _items.AddRange(new[] { Created, PickedUp, AwaitingApproval, Approved, ChangesRequested, Cancelled, Send });
        }

        protected MenuStatus() { }

        protected MenuStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}

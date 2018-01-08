using Axerrio.BB.DDD.Domain.DomainEvents;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;

namespace Axerrio.DDD.Menu.Domain.Events
{
    public class MenuCreatedDomainEvent : DomainEvent
    {
        public MenuAggr.Menu Menu { get; private set; }

        public MenuCreatedDomainEvent(MenuAggr.Menu menu)
        {
            Menu = EnsureArg.IsNotNull(menu, nameof(menu));
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Menu;
        }
    }
}

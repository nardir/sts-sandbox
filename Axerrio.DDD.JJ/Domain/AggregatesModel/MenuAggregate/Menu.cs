using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate
{
    public class Menu : Entity<int>,  IAggregateRoot
    {

        protected Menu()
        {
            //Create default things
        }

        public Menu(MenuStatus menuStatus, string description) : this()
        {
            MenuStatusId = menuStatus.Id;
            Description = description;

            AddDomainEvent(new MenuCreatedDomainEvent(this));
        }

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
        protected int MenuStatusId { get; set; }

        [JsonProperty]
        protected string Description { get; set; }

        [JsonIgnore]
 //       public MenuStatus MenuStatus => MenuStatus.Parse(MenuStatusId);
        public MenuStatus MenuStatus {
            get {
                return MenuStatus.Parse(MenuStatusId);
            }
            private set { }
        }

    }
}

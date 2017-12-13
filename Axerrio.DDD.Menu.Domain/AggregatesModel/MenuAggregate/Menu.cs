using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using Axerrio.DDD.Menu.Domain.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public Menu(MenuStatus menuStatus, string description, RequestInfo requestInfo) : this()
        {
            MenuStatusId = menuStatus.Id;
            Description = description;
            RequestInfo = requestInfo;

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

        [JsonProperty]
        public RequestInfo RequestInfo { get; set; }

        [JsonProperty]
        public Artist ArtistPickedUp { get; set; }       

        [JsonIgnore]       
        public MenuStatus MenuStatus
        {
            get
            {
                return MenuStatus.Parse(MenuStatusId);
            }
            private set
            {
                MenuStatusId = value.Id;
            }
        }

    }
}

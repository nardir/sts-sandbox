using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate
{
    public class Artist : Entity<int>, IAggregateRoot
    {
        protected Artist()
        {
            //Create default things
        }

        protected Artist(string firstName, string lastName, string emailAddress) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;

            AddDomainEvent(new ArtistCreatedDomainEvent(this));
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
        protected string FirstName { get; set; }
        [JsonProperty]
        protected string LastName { get; set; }
        [JsonProperty]
        protected string EmailAddress { get; set; }
    }
}

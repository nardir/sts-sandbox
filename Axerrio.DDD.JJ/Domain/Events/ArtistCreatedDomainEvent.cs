using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.Events
{
    public class ArtistCreatedDomainEvent : DomainEvent
    {
        public Artist Artist { get; private set; }

        public ArtistCreatedDomainEvent(Artist artist)
        {
            Artist = EnsureArg.IsNotNull(artist, nameof(artist));
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Artist;
        }
    }
}

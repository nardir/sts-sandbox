
using Axerrio.BB.DDD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate
{
    public  interface IArtistRepository : IRepository<Artist>
    {
        void Add(Artist artist); //todo, canncelationtoken!
        Task<List<Artist>> GetActiveArtistsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}

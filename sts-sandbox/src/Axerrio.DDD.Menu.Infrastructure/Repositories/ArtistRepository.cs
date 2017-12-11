using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Infrastructure.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        public ArtistRepository()
        {
        }

        private readonly MenuContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public ArtistRepository(MenuContext context)
        {
            _context = EnsureArg.IsNotNull(context);
        }

        public async Task<List<Artist>> GetAllAsync()
        {
            var artists = _context.Artist;
            return await artists.ToListAsync();
        }
    }
}

using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using EnsureThat;
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

    }
}

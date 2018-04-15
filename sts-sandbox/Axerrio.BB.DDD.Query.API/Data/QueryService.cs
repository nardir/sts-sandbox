using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Extensions;
using Axerrio.BB.DDD.Query.API.Model;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace Axerrio.BB.DDD.Query.API.Data
{
    public class QueryService : IQueryService
    {
        private readonly WorldWideImportersQueryContext _context;

        public QueryService(WorldWideImportersQueryContext context)
        {
            EnsureArg.IsNotNull(context, nameof(context));

            _context = context;
        }

        public async Task<IEnumerable<dynamic>> GetCustomersAsync(ISpecification<Customer> specification)
        {
            var query = _context.Customers
                .Include(c => c.CustomerCategory)
                .AsQueryable();

            query = query.ApplySpecification(specification);

            return await query.ToListAsync();
        }
    }
}

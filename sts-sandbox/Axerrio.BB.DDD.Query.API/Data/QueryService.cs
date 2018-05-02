using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Extensions;
using Axerrio.BB.DDD.Infrastructure.Query.Model;
using Axerrio.BB.DDD.Infrastructure.Query.Validation;
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

        public async Task<PagedEnumerable<Customer>> GetPagedCustomersAsync(ISpecification<Customer> specification)
        {
            //Validate specification
            var settings = new SpecificationValidationSettings
            {
                RequiredSpecificationOptions = SpecificationOptions.Ordering | SpecificationOptions.Paging,
                AllowedSpecificationOptions = SpecificationOptions.Filter | SpecificationOptions.Ordering | SpecificationOptions.Paging
            };

            specification.Validate(settings);

            //if (!specification.HasPaging)
            //    return null;

            var baseQuery = _context.Customers
                .Include(c => c.CustomerCategory)
                .AsQueryable();

            if (specification.HasPredicate)
                baseQuery = baseQuery.ApplySpecificationPredicate(specification);

            var itemCount = await baseQuery.LongCountAsync();

            var query = baseQuery
                .ApplySpecificationOrdering(specification)
                .ApplySpecificationPaging(specification);

            var customers = await query.ToListAsync();

            var pagedCustomers = PagedEnumerable.Create(customers, specification, itemCount);

            return pagedCustomers;
        }

        public async Task<PagedEnumerable<dynamic>> GetPagedCustomersAsync2(ISpecification<Customer> specification)
        {
            //Validate specification
            if (!specification.HasPaging)
                return null;

            var baseQuery = _context.Customers
                .Include(c => c.CustomerCategory)
                .AsQueryable();

            var countQuery = baseQuery;
            if (specification.HasPredicate)
                countQuery = countQuery.ApplySpecificationPredicate(specification);

            var itemCount = await countQuery.LongCountAsync();

            var query = baseQuery.ApplySpecification<Customer, dynamic>(specification);

            var customers = await query.ToListAsync();

            var pagedCustomers = PagedEnumerable.Create(customers, specification, itemCount);

            return pagedCustomers;
        }
    }
}

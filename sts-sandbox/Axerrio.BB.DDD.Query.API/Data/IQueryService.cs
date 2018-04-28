using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Model;
using Axerrio.BB.DDD.Query.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Data
{
    public interface IQueryService
    {
        Task<IEnumerable<dynamic>> GetCustomersAsync(ISpecification<Customer> specification);
        Task<PagedEnumerable<Customer>> GetPagedCustomersAsync(ISpecification<Customer> specification);
        Task<PagedEnumerable<dynamic>> GetPagedCustomersAsync2(ISpecification<Customer> specification);
    }
}
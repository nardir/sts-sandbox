using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public interface ISalesQueries
    {
        Task<IEnumerable<SalesOrder>> SalesOrders();
    }
}

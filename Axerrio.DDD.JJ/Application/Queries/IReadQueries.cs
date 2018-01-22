using Moon.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Queries
{
    public interface IReadQueries
    {
        Task<IEnumerable<dynamic>> QueryWithODataOptionsAsync<T>(string sqlQuery, ODataOptions<T> oDataOptions) where T : QueryDTO;
    }
}

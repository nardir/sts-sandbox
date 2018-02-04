using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class SalesQueries : ISalesQueries
    {
        private readonly string _connectionString;

        public SalesQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<SalesOrder>> SalesOrders()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync<SalesOrder>(@"select top 10 o.* from Sales.Orders as o");
            }
        }
    }
}

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

        //public async Task<IEnumerable<dynamic>> SalesOrdersAsync()
        public async Task<dynamic> SalesOrdersAsync()
        {
            var arguments = new List<object>();
            arguments.Add("Wingtip Toys");
            arguments.Add(DateTime.Parse("2016-05-31"));
            arguments.Add(32M);
            //arguments.Add(2.4M);
            //arguments.Add(true);

            var parameters1 = new DynamicParameters();

            for (var i = 0; i < arguments.Count; i++)
            {
                parameters1.Add($"p{i}", arguments[i]);
            }

            var parameters = new DynamicParameters(parameters1);
            parameters.AddDynamicParams(new { IsUndersupplyBackordered = true });

            var sql = @"select c.CustomerName, o.*
                        from Sales.Orders as o
                        join Sales.Customers as c
                        on o.CustomerID = c.CustomerID
                        where c.CustomerName like '%' + @p0 + '%'
                        and o.OrderDate = @p1
                        and o.IsUndersupplyBackordered = @IsUndersupplyBackordered
                        and exists
                        (
                            select 1
                            from Sales.OrderLines as l
                            where l.OrderID = o.OrderID
                            and UnitPrice = @p2
                        )
                        ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var salesOrders = await connection.QueryAsync<dynamic>(sql, parameters);

                if (salesOrders.AsList().Count() > 0)
                {
                    dynamic orders = salesOrders;

                    var salesOrder = orders[0];

                    var orderId = salesOrder.OrderID;
                }

                return salesOrders;
            }
        }
    }
}

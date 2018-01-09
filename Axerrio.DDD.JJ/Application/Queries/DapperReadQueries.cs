using Axerrio.DDD.Menu.Application.DTOs;
using Dapper;
using EnsureThat;
using Microsoft.Extensions.Options;
using Moon.OData;
using Moon.OData.Sql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Queries
{
    //Todo: naamgevingen
    public class DapperReadQueries : IReadQueries
    {
        private readonly ReadQueryOptions _readQueryOptions;
        private readonly string _connectionString = string.Empty;

        public DapperReadQueries(ReadQueryOptions readQueryOptions)  
        {
            _readQueryOptions = EnsureArg.IsNotNull(readQueryOptions, nameof(readQueryOptions));    
            _connectionString = EnsureArg.IsNotNullOrWhiteSpace(_readQueryOptions.ConnectionString, nameof(_readQueryOptions.ConnectionString)); 
        }       
               
        public async Task<IEnumerable<T>> QueryWithODataOptionsAsync<T>(string sqlQuery, ODataOptions<T> oDataOptions)
                where T : QueryDTO
        {
            EnsureArg.IsNotNullOrWhiteSpace(sqlQuery);

          //  var moonSqlQuery = new ODataSqlQuery(
          //     "SELECT FROM XXX", // Geen collomen, dan werkt select ook!
          //     oDataOptions
          // );

          //  var moonSqlQuery2 = new ODataSqlQuery( // Wel kollommen, dan werk $select niet, pakt hij altijd gespecifieerde kolommen!
          //     "select m.Description as Menu, isnull(a.FirstName + ' ' + a.LastName, 'None') as Artist, ms.Name as [Status] from dbo.Menus m left join dbo.Artist a on m.ArtistId = a.ArtistId join [dbo].[MenuStatus] ms on ms.MenuStatusId = m.MenuStatusId",
          //     oDataOptions
          // );

          //  var moonSqlQuery3 = new ODataSqlQuery(
          //    @"SELECT 
          //          FROM 
          //              XXX",
          //    oDataOptions
          //); // ERROR ERROR ERROR

          //  var moonSqlQuery4 = new ODataSqlQuery(
          //    @"SELECT FROM (SELCT '',2   FROM  XXX) As AAA"
          //      ,     oDataOptions
          //);                 

            var pureSqlQueryString = Regex.Replace(sqlQuery, @"\t|\n|\r", "");
            var inlineQuery = "SELECT FROM (" + pureSqlQueryString + " ) " + typeof(T).Name;
            //Inline nodig, want bv WHERE op [Status] gaat anders fout!!

            var odataSqlQuery = new ODataSqlQuery(
                    inlineQuery,
                    oDataOptions
            );

            //Parsen testen... 
            var parsedCommandText = odataSqlQuery.CommandText;
            var argumentsList = odataSqlQuery.Arguments;

            DynamicParameters parameters = new DynamicParameters();
            
            var i = 0;
            foreach (var argument in argumentsList)
            {
                parameters.Add(name: "@p" + i++, value: argument);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<T>(
                    parsedCommandText,
                    parameters);

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return result;
            }
        }
    }
}

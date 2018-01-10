using Microsoft.AspNetCore.Mvc;
using Moon.OData;
using Moon.OData.Sql;
using System.Text.RegularExpressions;

namespace Axerrio.DDD.Menu.Controllers
{
    public class Entity
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    [Route("api/moon")]        
    public class MooningController : Controller
    {
        //http://localhost:49675/api/moon?$count=true&$select=Name,Id&$filter=Id%20eq%201&$orderby=Name%20desc&$top=2&$skip=1
        //xxx/api/moon?$count=true&$select=Name,Id&$filter=Id%20eq%201&$orderby=Name%20desc&$top=2&$skip=1
        [HttpGet("")]
        public IActionResult Index(ODataOptions<Entity> options) 
        {
            var moonSqlQuery = new ODataSqlQuery(
                "SELECT FROM Entities WHERE OwnerId = @p0",
                10456, options
            );

            var skip = options.Skip;
            var top = options.Top;

            var sqlString = @"select m.Description as Menu,
                        isnull(a.FirstName + ' ' + a.LastName, 'None') as Artist,
                        ms.Name as [Status]
                    from dbo.Menus m
                    left
                    join dbo.Artist a

                   on m.ArtistId = a.ArtistId
                    join [dbo].[MenuStatus] ms

                        on ms.MenuStatusId = m.MenuStatusId";
            var moon2SqlQuery = new ODataSqlQuery(
                sqlString,
                options
            );

            var pureSqlQueryString = Regex.Replace(sqlString, @"\t|\n|\r", "");
            moon2SqlQuery = new ODataSqlQuery(
                sqlString, options
            );


            var moon3SqlQuery = new ODataSqlQuery(
                @"SELECT FROM (select m.Description as Id, ms.Name as Name from dbo.Menus m left join dbo.Artist a on m.ArtistId = a.ArtistId join [dbo].[MenuStatus] ms on ms.MenuStatusId = m.MenuStatusId) Entities",
                options
            );
          

            var parsedCommandText = moonSqlQuery.CommandText;
            var arguments = moonSqlQuery.Arguments;

            if (moonSqlQuery.Count != null)
            {
                var countfilterwithoutSkipandTake = moonSqlQuery.Count.CommandText;
                var countfilterwithoutSkipandTakearguments = moonSqlQuery.Count.Arguments;
            }


            return Ok(new ODataSqlQuery(
                "SELECT FROM Entities WHERE OwnerId = @p0",
                10456, options
            ));
        }
    }
}
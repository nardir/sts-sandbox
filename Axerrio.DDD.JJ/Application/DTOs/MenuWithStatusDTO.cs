using Axerrio.DDD.Menu.Application.Queries;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.DTOs
{
    public class ReadQueryDefinition<T>
    {
        public ReadQueryDefinition(string sqlQuery)
        {
            SqlQuery = EnsureArg.IsNotNullOrWhiteSpace(sqlQuery);
            DTOType = typeof(T);
        }

        public string SqlQuery { get; private set; }
        public Type DTOType { get; private set; }
    }

    public class MenuReadQueries
    {
        public static readonly ReadQueryDefinition<MenuWithStatusDTO> MenuWithStatusQuery = new ReadQueryDefinition<MenuWithStatusDTO>(
            @"select 
	                    m.Description as Menu,
	                    isnull(a.FirstName + ' ' + a.LastName, 'None') as Artist,
	                    ms.Name as [Status]
                    from dbo.Menus m 
                    left join dbo.Artist a
	                    on m.ArtistId = a.ArtistId
                    join [dbo].[MenuStatus] ms
	                    on ms.MenuStatusId = m.MenuStatusId"
        );   
    }  

    //Queries integratie testen op bestaande db?    
    public class MenuWithStatusDTO : QueryDTO
    {

        public string Menu { get; set; }
        public string Artist { get; set; }
        public string Status { get; set; }
    }

    public class MenuWithStatusDTO_v1801 : QueryDTO
    {    

        public string Menu { get; set; }
        public string Status { get; set; }
    }


    public class MenuReadQueries_v1801
    {
        public static readonly ReadQueryDefinition<MenuWithStatusDTO_v1801> MenuWithStatusQuery = new ReadQueryDefinition<MenuWithStatusDTO_v1801>(
            @"select 
	                    m.Description as Menu,
	                    ms.Name as [Status]
                    from dbo.Menus m 
                    left join dbo.Artist a
	                    on m.ArtistId = a.ArtistId
                    join [dbo].[MenuStatus] ms
	                    on ms.MenuStatusId = m.MenuStatusId"
        );
    }
}

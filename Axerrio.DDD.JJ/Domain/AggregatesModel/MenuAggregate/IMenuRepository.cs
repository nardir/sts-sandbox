using Axerrio.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu> GetAsync(int menuId);
        void Add(Menu menu);
        void Update(Menu menu);        
    }
}

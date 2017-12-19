using Axerrio.BB.DDD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu> GetAsync(int menuId);
        Menu Add(Menu menu);
        void Update(Menu menu);        
    }
}

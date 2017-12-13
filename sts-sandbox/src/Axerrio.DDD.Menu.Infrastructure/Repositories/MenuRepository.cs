using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;

namespace Axerrio.DDD.Menu.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MenuContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public MenuRepository(MenuContext context)
        {
            _context = EnsureArg.IsNotNull(context);
        }

        public async Task<MenuAggr.Menu> GetAsync(int menuId)
        {
            var menu = await _context.Menu.FindAsync(menuId);
            if (menu != null)
            {      
                await _context.Entry(menu)
                    .Reference(i => i.RequestInfo).LoadAsync();
            }

            return menu;
        }

        public void Add(MenuAggr.Menu menu)
        {
            _context.Menu.Add(menu);
        }

        public void Update(MenuAggr.Menu menu)
        {
            _context.Entry(menu).State = EntityState.Modified;
        }
    }
}

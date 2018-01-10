using Axerrio.DDD.Menu.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Axerrio.DDD.Menu.Infrastructure.ContextDesignFactory
{
    public class MenuContextFactory : IDesignTimeDbContextFactory<MenuContext>
    {
        public MenuContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MenuContext>()
                .UseSqlServer("Server=(localdb)\\ProjectsV12;Initial Catalog=DataStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return new MenuContext(optionsBuilder.Options);
        }
    }
}
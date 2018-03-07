using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class WorldWideImportersQueryContext: DbContext
    {
        public DbQuery<WebCustomer> WebCustomers { get; set; }

        public WorldWideImportersQueryContext(DbContextOptions<WorldWideImportersQueryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<WebCustomer>()
                .ToTable("Customers", "Website");
        }
    }
}

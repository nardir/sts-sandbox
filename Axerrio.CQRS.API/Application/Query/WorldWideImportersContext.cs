using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class WorldWideImportersContext: DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public WorldWideImportersContext(DbContextOptions<WorldWideImportersContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .ToTable("Customers", "Sales");

            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .IsRequired(true);

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerID);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                .HasColumnName("CustomerName");

        }
    }
}

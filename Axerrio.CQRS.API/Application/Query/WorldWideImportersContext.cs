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
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderLine> SalesOrderLines { get; set; }

        //public DbQuery<WebCustomer> WebCustomers { get; set; }

        public WorldWideImportersContext(DbContextOptions<WorldWideImportersContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Query<WebCustomer>()
            //    .ToTable("Customers", "Website");

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

            modelBuilder.Entity<SalesOrder>()
                .ToTable("Orders", "Sales");

            modelBuilder.Entity<SalesOrder>()
                .Property(so => so.OrderID)
                .IsRequired(true);

            modelBuilder.Entity<SalesOrder>()
                .HasKey(so => so.OrderID);

            modelBuilder.Entity<SalesOrder>()
                .HasOne(so => so.Customer)
                .WithMany()
                .HasForeignKey(so => so.CustomerID);

            modelBuilder.Entity<SalesOrder>()
                .HasMany(so => so.SalesOrderLines)
                .WithOne()
                .HasForeignKey(l => l.OrderID);

            modelBuilder.Entity<SalesOrderLine>()
                .ToTable("OrderLines", "Sales");

            modelBuilder.Entity<SalesOrderLine>()
                .HasKey(l => l.OrderLineID);
        }
    }
}

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

        public DbSet<CustomerCategory> CustomerCategories { get; set; }

        //public DbQuery<WebCustomer> WebCustomers { get; set; }

        public WorldWideImportersContext(DbContextOptions<WorldWideImportersContext> options)
            : base(options)
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Query<WebCustomer>()
            //    .ToTable("Customers", "Website");


            //https://romiller.com/2017/02/14/ef-core-1-1-read-only-entities-extending-metadata-with-annotations/
            //https://www.learnentityframeworkcore.com/configuration/data-annotation-attributes
            modelBuilder.Entity<Customer>()
                .HasAnnotation("custom:url", "api/sales/customers");

            modelBuilder.Entity<Customer>()
                .ToTable("Customers", "Sales");

            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .IsRequired(true);

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerID);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                .HasColumnName("CustomerName")
                .HasAnnotation("custom:supportslike", true);

            modelBuilder.Entity<Customer>()
                .Property(c => c.CreditLimit)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Customer>()
                .Property(c => c.AccountOpenedDate)
                .HasColumnType("date");


            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerCategory)
                .WithMany()
                .HasForeignKey(c => c.CustomerCategoryID);

            modelBuilder.Entity<CustomerCategory>()
                .ToTable("CustomerCategories", "Sales");

            modelBuilder.Entity<CustomerCategory>()
                .HasKey(cc => cc.CustomerCategoryID);

            ///////

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
                .WithOne(l => l.SalesOrder)
                .HasForeignKey(l => l.OrderID);

            modelBuilder.Entity<SalesOrderLine>()
                .ToTable("OrderLines", "Sales");

            modelBuilder.Entity<SalesOrderLine>()
                .HasKey(l => l.OrderLineID);
        }
    }
}

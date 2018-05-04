using Axerrio.BB.DDD.Infrastructure.Query.EntityFrameworkCore;
using Axerrio.BB.DDD.Query.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Axerrio.BB.DDD.Query.API.Data
{

    public class WorldWideImportersQueryContext: QueryDbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerCategory> CustomerCategories { get; set; }

        public WorldWideImportersQueryContext(DbContextOptions<WorldWideImportersQueryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Customer

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

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.SalesOrders)
                .WithOne()
                .HasForeignKey(c => c.CustomerID);

            modelBuilder.Entity<Customer>()
                .Ignore(c => c.SalesOrderCount);

            #endregion

            #region SalesOrder

            modelBuilder.Entity<SalesOrder>()
                .ToTable("Orders", "Sales");

            modelBuilder.Entity<SalesOrder>()
                .HasKey(so => so.OrderID);

            modelBuilder.Entity<SalesOrder>()
                .Property(so => so.OrderDate)
                .IsRequired();

            #endregion

            #region CustomerCategory

            modelBuilder.Entity<CustomerCategory>()
                .ToTable("CustomerCategories", "Sales");

            modelBuilder.Entity<CustomerCategory>()
                .HasKey(cc => cc.CustomerCategoryID);

            #endregion
        }
    }
}

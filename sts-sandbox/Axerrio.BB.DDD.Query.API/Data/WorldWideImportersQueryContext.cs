using Axerrio.BB.DDD.Query.API.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Data
{
    public abstract class QueryDbContext: DbContext
    {
        public QueryDbContext(DbContextOptions options): base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("Query/readonly DbContext");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(SaveChanges());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(SaveChanges());
        }
    }

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

            modelBuilder.Entity<CustomerCategory>()
                .ToTable("CustomerCategories", "Sales");

            modelBuilder.Entity<CustomerCategory>()
                .HasKey(cc => cc.CustomerCategoryID);
        }
    }
}

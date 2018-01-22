using Axerrio.DDD.BuildingBlocks;
using Axerrio.DDD.Ordering.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Axerrio.DDD.Ordering.Data
{
    public class OrderingContext: DbContext, IUnitOfWork
    {
        //public DbSet<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }
        public DbSet<PaymentMethod> Paymentmethods { get; set; }

        public OrderingContext(DbContextOptions<OrderingContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEventLogEntryConfiguration());

            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
        }

        #region IUnitOfWork

        public Task<bool> DispatchDomainEventsAndSaveChangesAsync(bool saveChanges = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        #endregion IUnitOfWork
    }
}
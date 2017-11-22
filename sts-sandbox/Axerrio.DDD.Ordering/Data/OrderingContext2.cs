using Axerrio.DDD.BuildingBlocks;
using Axerrio.DDD.Ordering.Model;
using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Data
{
    public class OrderingContext2: IntegrationEventLogContext, IUnitOfWork
    {
        public DbSet<PaymentMethod> Paymentmethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        private readonly IMediator _mediator;

        private OrderingContext2(DbContextOptions<OrderingContext2> options) : base(options) { }

        public OrderingContext2(DbContextOptions<OrderingContext2> options, IMediator mediator): base(options)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        }

        #region IUnitOfWork

        public Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _mediator.DispatchDomainEventsAsync(this);
        }

        public Task<bool> DispatchDomainEventsAndSaveChangesAsync(bool saveChanges = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        #endregion IUnitOfWork
    }
}

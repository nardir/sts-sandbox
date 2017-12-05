using Axerrio.BuildingBlocks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.DDD.Menu.Infrastructure.EntityTypeConfigurations;
using MediatR;
using EnsureThat;

namespace Axerrio.DDD.Menu.Infrastructure
{
    public class MenuContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "menu";

        public DbSet<MenuStatus> MenuStatus { get; set; }

        private readonly IMediator _mediator;

        private MenuContext(DbContextOptions<MenuContext> options) : base (options) { }

        public MenuContext(DbContextOptions<MenuContext> options, IMediator mediator): base(options)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MenuStatusEntityTypeConfiguration());
        }

        #region IUnitOfWork
        public async Task<bool> DispatchDomainEventsAndSaveChangesAsync(bool saveChanges = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);

            if (saveChanges)
                await SaveChangesAsync();

            return true;
        }

        #endregion
    }
}

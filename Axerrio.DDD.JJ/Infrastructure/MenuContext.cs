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
using Microsoft.EntityFrameworkCore.Design;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;

namespace Axerrio.DDD.Menu.Infrastructure
{
    public class MenuContextDesignFactory : IDesignTimeDbContextFactory<MenuContext>
    {
        public MenuContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MenuContext>()
                .UseSqlServer("Server=(localdb)\\ProjectsV12;Initial Catalog=DataStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return new MenuContext(optionsBuilder.Options);
        }
    }

    public class MenuContext : BaseContext //: DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "dbo";

        public DbSet<MenuStatus> MenuStatus { get; set; }
        public DbSet<MenuAggr.Menu> Menu { get; set; }

        private readonly IMediator _mediator;

        public MenuContext(DbContextOptions<MenuContext> options) : base (options) { }

        public MenuContext(DbContextOptions<MenuContext> options, IMediator mediator): base(options)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MenuStatusEntityTypeConfiguration(DEFAULT_SCHEMA));
            modelBuilder.ApplyConfiguration(new MenuEntityTypeConfiguration(DEFAULT_SCHEMA));            
        }        
    }

    public abstract class BaseContext : DbContext, IUnitOfWork
    {
        protected readonly IMediator _mediator;

        public BaseContext(DbContextOptions<MenuContext> options) : base (options) { }

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

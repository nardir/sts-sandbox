using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    //Using Microsoft.EntityFrameworkCore --> Daarom infrastructure?

    public abstract class DbContextUnitOfWork : DbContext, IUnitOfWork
    {
        protected readonly IMediator _mediator;
        public DbContextUnitOfWork(DbContextOptions options) : base(options){}
        public DbContextUnitOfWork(DbContextOptions options, IMediator mediator) : base(options)
        {
            _mediator = EnsureArg.IsNotNull(mediator, nameof(mediator));
        }
        
        public async Task<bool> DispatchDomainEventsAndSaveChangesAsync(bool saveChanges = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);

            if (saveChanges)
                await SaveChangesAsync();

            return true;
        }      
        
    }
}

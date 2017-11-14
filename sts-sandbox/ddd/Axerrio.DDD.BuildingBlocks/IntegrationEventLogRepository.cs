using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public class IntegrationEventLogRepository<TContext> : IIntegrationEventLogRepository
        where TContext : DbContext, IUnitOfWork
    {
        private readonly TContext _context;

        public IntegrationEventLogRepository(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => _context;

        public void Add(IntegrationEvent @event)
        {
            _context.Add(new IntegrationEventLogEntry(@event));
        }
    }
}

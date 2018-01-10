using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class IntegrationEventsEnqueueService<TContext> : IIntegrationEventsEnqueueService
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<IntegrationEventsEnqueueService<TContext>> _logger;

        public IntegrationEventsEnqueueService(TContext context, ILogger<IntegrationEventsEnqueueService<TContext>> logger)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));

            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            //_context.Database.GetDbConnection().ConnectionString
        }

        public async Task EnqueueAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            //TODO NR : Check of Async nodig is
            EnsureArg.IsNotNull(@event, nameof(@event));

            _context.IntegrationEventsQueueItems.Add(new IntegrationEventsQueueItem(@event));

            await Task.CompletedTask;
        }
    }
}
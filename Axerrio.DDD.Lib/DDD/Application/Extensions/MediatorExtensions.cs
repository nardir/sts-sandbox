using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;

using System.Linq;

using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<IDomainEventsEntity>()
                .Where(e => e.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }      

    }
}

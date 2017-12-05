using EnsureThat;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public static class MediatorExtension
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

        public static Task SendCommandAsync(this IMediator mediator, ICommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRequest request = command;

            return mediator.Send(request, cancellationToken);
        }

        public static Task SendCommandAsync<TResponse>(this IMediator mediator, ICommand<TResponse> command, CancellationToken cancellationToken = default(CancellationToken))
        {
            IRequest<TResponse> request = command;

            return mediator.Send(request, cancellationToken);
        }
    }
}

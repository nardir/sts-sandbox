using MediatR;

namespace Axerrio.BuildingBlocks
{
    public interface IDomainEventHandler<TDomainEvent>: IAsyncNotificationHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {
    }
}

using MediatR;

namespace Axerrio.BuildingBlocks
{
    public interface IDomainEventHandler<TDomainEvent>: INotificationHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {
    } 
}

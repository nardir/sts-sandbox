using MediatR;

namespace Axerrio.BuildingBlocks
{
    public abstract class DomainEvent: ValueObject<DomainEvent>, INotification
    {

    }
}

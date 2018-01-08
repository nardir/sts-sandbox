using MediatR;

namespace Axerrio.BuildingBlocks
{
    //Mediatr --> aparte nuget om die dependency?

    public abstract class DomainEvent: ValueObject<DomainEvent>, INotification
    {

    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.Lib
{
    public interface IDomainEventHandler<TDomainEvent>: IAsyncNotificationHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {
    }
}

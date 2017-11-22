using Axerrio.DDD.BuildingBlocks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.Lib
{
    public abstract class DomainEvent: ValueObject<DomainEvent>, INotification
    {
    }
}

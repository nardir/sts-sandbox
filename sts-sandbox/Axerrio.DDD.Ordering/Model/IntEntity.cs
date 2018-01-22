using Axerrio.DDD.BuildingBlocks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.Model
{
    public abstract class IntEntity: Entity<int, INotification>
    {
    }
}

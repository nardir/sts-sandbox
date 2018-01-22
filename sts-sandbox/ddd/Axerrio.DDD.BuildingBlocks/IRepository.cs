using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

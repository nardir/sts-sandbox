using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public interface IRepository<T> where T : IAggregateRoot, IEntity
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

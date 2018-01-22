using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BuildingBlocks
{
    public interface IIdentifiedCommand<TCommand>
    {
        TCommand Command { get; }
        Guid Id { get; }
    }
}

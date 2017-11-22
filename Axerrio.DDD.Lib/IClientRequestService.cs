using Axerrio.DDD.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.BuildingBlocks
{
    public interface IClientRequestService
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateClientRequestForCommandAsync<TCommand>(Guid id) where TCommand : ICommand;
    }
}

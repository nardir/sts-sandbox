using System;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public interface IClientRequestService
    {
        Task<bool> ExistAsync(Guid id);
        Task CreateClientRequestForCommandAsync<TCommand>(Guid id) where TCommand : ICommand;
    }
}

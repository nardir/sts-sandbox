using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BuildingBlocks
{
    public interface IClientRequestService
    {
        Task<bool> ExistAsync(Guid id);
        Task CreateClientRequestForCommandAsync<TCommand>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;
        Task CreateClientRequestForCommandAsync<TCommand, TResponse>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand<TResponse>;

    }
}

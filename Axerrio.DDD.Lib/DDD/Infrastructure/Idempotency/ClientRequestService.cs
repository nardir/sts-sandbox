using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Axerrio.BuildingBlocks
{
    public class ClientRequestService : IClientRequestService
    {
        private readonly ClientRequestContext _context;
        private readonly ILogger<ClientRequestContext> _logger;

        public ClientRequestService(ClientRequestContext context, ILogger<ClientRequestContext> logger)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        private async Task CreateClientRequestForCommandAsync(Guid id, Type commandType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await ExistAsync(id))
                throw new DomainException($"Request with {id} already exists");
        

            var request = new ClientRequest(id, commandType.Name, DateTime.UtcNow);

            await _context.AddAsync(request, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug($"{request.ToString()}"); //todo: better info!
        }

        public Task CreateClientRequestForCommandAsync<TCommand, TResponse>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand<TResponse>
        {
            return CreateClientRequestForCommandAsync(id, typeof(TCommand), cancellationToken);
        }
 
        public Task CreateClientRequestForCommandAsync<TCommand>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand
        {
            return CreateClientRequestForCommandAsync(id, typeof(TCommand), cancellationToken);
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.FindAsync<ClientRequest>(id);

            return request != null;
        }
    }
}

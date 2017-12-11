using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;

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
        public async Task CreateClientRequestForCommandAsync<TCommand>(Guid id) where TCommand : ICommand
        {
            if (await ExistAsync(id))
            {
                var domainException = new DomainException($"Request with {id} already exists");
                _logger.LogError(domainException, $"Request with {id} already exists");

                throw domainException;
            }

            var request = new ClientRequest(id, typeof(TCommand).Name, DateTime.UtcNow);

            _context.Add(request);

            await _context.SaveChangesAsync();

            _logger.LogDebug($"{request.ToString()}");
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.FindAsync<ClientRequest>(id);

            return request != null;
        }
    }
}

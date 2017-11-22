using Axerrio.DDD.Lib;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Ordering.BuildingBlocks
{
    public class ClientRequestService : IClientRequestService
    {
        private readonly ClientRequestContext _context;

        public ClientRequestService(ClientRequestContext context)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
        }
        public async Task CreateClientRequestForCommandAsync<TCommand>(Guid id) where TCommand : ICommand
        {
            if (await ExistAsync(id))
                throw new DomainException($"Request with {id} already exists");

            var request = new ClientRequest(id, typeof(TCommand).Name, DateTime.UtcNow);

            _context.Add(request);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.FindAsync<ClientRequest>(id);

            return request != null;
        }
    }
}

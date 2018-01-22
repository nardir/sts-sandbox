using Axerrio.BB.DDD.Application.Commands;
using Axerrio.BB.DDD.Infrastructure.Idempotency;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Commands
{
    public class AddArtistCommandHandler : CommandHandler<AddArtistCommand>
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<AddArtistCommandHandler> _logger;

        public AddArtistCommandHandler(IArtistRepository artistRepository, ILogger<AddArtistCommandHandler> logger)
        {
            _artistRepository = EnsureArg.IsNotNull(artistRepository);
            _logger = EnsureArg.IsNotNull(logger);
        }

        public override async Task Handle(AddArtistCommand message, CancellationToken cancellationToken = default(CancellationToken))
        {
            //todo: menu props erbij!
            var artist = new Artist(message.FirstName, message.LastName, message.EmailAddress);
            _artistRepository.Add(artist);

            await _artistRepository.UnitOfWork.DispatchDomainEventsAndSaveChangesAsync(message.Initiating, cancellationToken);           
        }
        
    }

    public class AddArtistIdentifiedCommandHandler : IdentifiedCommandHandler<AddArtistCommand>
    {
        public AddArtistIdentifiedCommandHandler(IMediator mediator, IClientRequestService clientRequestService) : base(mediator, clientRequestService)
        {
        }        
    }
}

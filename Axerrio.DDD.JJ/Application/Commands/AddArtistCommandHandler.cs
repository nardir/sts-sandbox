using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Commands
{
    public class AddArtistCommandHandler : ICommandHandler<AddArtistCommand>
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<AddArtistCommandHandler> _logger;

        public AddArtistCommandHandler(IArtistRepository artistRepository, ILogger<AddArtistCommandHandler> logger)
        {
            _artistRepository = EnsureArg.IsNotNull(artistRepository);
            _logger = EnsureArg.IsNotNull(logger);
        }

        public async Task Handle(AddArtistCommand message)
        {
            //todo: menu props erbij!
            var artist = new Artist(message.FirstName, message.LastName, message.EmailAddress);
            _artistRepository.Add(artist);

            if (message.Initiating)
                await _artistRepository.UnitOfWork.DispatchDomainEventsAndSaveChangesAsync();

           
        }
    }
}

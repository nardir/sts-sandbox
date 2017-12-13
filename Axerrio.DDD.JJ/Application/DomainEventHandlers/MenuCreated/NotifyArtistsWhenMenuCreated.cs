using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Domain.AggregatesModel.ArtistAggregate;
using Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.DDD.Menu.Domain.Events;
using EnsureThat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Axerrio.DDD.Menu.Application.DomainEventHandlers.MenuCreated
{
    public class NotifyArtistsWhenMenuCreated : DomainEventHandler<MenuCreatedDomainEvent>
    {
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<NotifyArtistsWhenMenuCreated> _logger;

        public NotifyArtistsWhenMenuCreated(IArtistRepository artistRepository, ILogger<NotifyArtistsWhenMenuCreated> logger)
        {
            _artistRepository = EnsureArg.IsNotNull(artistRepository);
            _logger = EnsureArg.IsNotNull(logger);
        }

        public override async Task Handle(MenuCreatedDomainEvent notification, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Create Command?
            
            var artists = await _artistRepository.GetActiveArtistsAsync(cancellationToken);

            string emailTo = "";
            artists.ForEach(artist => emailTo += artist.GetEmailAddress() + ";");

            //Send Mail.
            //Use service of integration event.
            _logger.LogDebug("Email Send to: " + emailTo);
            
        }
        
    }
}

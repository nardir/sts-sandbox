using Axerrio.BuildingBlocks;
using Axerrio.DDD.Menu.Application.Commands;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Controllers
{
    [Route("api/artist")]
    public class ArtistController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ArtistController> _logger;

        public ArtistController(IMediator mediator, ILogger<ArtistController> logger)
        {
            _mediator = EnsureArg.IsNotNull(mediator);
            _logger = EnsureArg.IsNotNull(logger);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> AddArtist([FromBody]AddArtistCommand addArtistCommand, [FromHeader(Name = "x-requestid")] string requestId)
        {           
            bool commandResult = false;            
            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestAddArtist = new IdentifiedCommand<AddArtistCommand>(addArtistCommand, guid);  
                await _mediator.SendCommandAsync(requestAddArtist);

                commandResult = true;
            }

            return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();  
            
            
        }
    }
}

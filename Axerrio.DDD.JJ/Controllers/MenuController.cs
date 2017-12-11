using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using EnsureThat;
using Axerrio.DDD.Menu.Application.Commands;
using Axerrio.BuildingBlocks;
using Microsoft.Extensions.Logging;

namespace Axerrio.DDD.Menu.Controllers
{    
    [Route("api/menu")]
    public class MenuController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMediator mediator, ILogger<MenuController> logger)
        {
            _mediator = EnsureArg.IsNotNull(mediator);
            _logger = EnsureArg.IsNotNull(logger);
        }

        [Route("submit")]
        [HttpPost]
        public async Task<IActionResult> SubmitMenu([FromBody]SubmitMenuCommand submitMenuCommand, [FromHeader(Name = "x-requestid")] string requestId)
        {
            try
            {
                _logger.LogInformation("Testje! SubmitMenu.");

                bool commandResult = false;

                //Todo: Parsing + Identifiedcommand in Extension Method?
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var requestSubmitMenu = new IdentifiedCommand<SubmitMenuCommand, bool>(submitMenuCommand, guid); 
                    //Todo: Test, Exception indien geen corresponderende handler gevonden ...
                    //--> Iedere Command een test?

                    commandResult = await _mediator.SendCommandAsync(requestSubmitMenu);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
                
            }
            catch(Exception ex)
            {
                //TODO: Algemene catch ofzo?
                return (IActionResult)BadRequest(ex);
            }
        }


    }
}
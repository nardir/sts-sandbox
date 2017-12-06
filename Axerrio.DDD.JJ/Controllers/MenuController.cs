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

namespace Axerrio.DDD.Menu.Controllers
{    
    [Route("api/menu")]
    public class MenuController : Controller
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = EnsureArg.IsNotNull(mediator);
        }

        [Route("submit")]
        [HttpPost]
        public async Task<IActionResult> SubmitMenu([FromBody]SubmitMenuCommand submitMenuCommand, [FromHeader(Name = "x-requestid")] string requestId)
        {
            try
            {
                bool commandResult = false;
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var requestSubmitMenu = new IdentifiedCommand<SubmitMenuCommand, bool>(submitMenuCommand, guid);
               
                    commandResult = await _mediator.SendCommandAsync(requestSubmitMenu);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch(Exception ex)
            {

                return (IActionResult)BadRequest(ex);
            }
        }


    }
}
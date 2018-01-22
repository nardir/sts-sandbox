using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using EnsureThat;
using Axerrio.DDD.Menu.Application.Commands;
using Microsoft.Extensions.Logging;
using MenuAggr = Axerrio.DDD.Menu.Domain.AggregatesModel.MenuAggregate;
using Axerrio.BB.DDD.Application.Commands;
using Moon.OData;
using Axerrio.DDD.Menu.Application.Queries;
using Axerrio.DDD.Menu.Application.DTOs;

namespace Axerrio.DDD.Menu.Controllers
{    
    [Route("api/menu")]
    public class MenuController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;
        private readonly IReadQueries _readQueries;

        public MenuController(IMediator mediator, ILogger<MenuController> logger, IReadQueries readQueries)
        {
            _mediator = EnsureArg.IsNotNull(mediator);
            _logger = EnsureArg.IsNotNull(logger);
            _readQueries = EnsureArg.IsNotNull(readQueries);
        }

        //Test odata strings:
        //http://localhost:49675/api/menu/?$filter=Status%20eq%20%27Created%27&$orderby=Menu%20desc&$top=2
        //localhost:49675/api/menu/?$filter=Status eq 'Created'&$orderby=Menu desc&$top=2&$select=Menu&$take=1

        [Route("")]
        [HttpGet]
        //TODO: Swagger definitie Filter oid?
        //1. In swagger queryoptions in kunnen geven? todo: Kijk naar swagger odata extensions nuget.
        //2. Nu breekt swagger op ODataOptions wanneer schemafilter aanstaat voor validatie (fluent)--> //options.SchemaFilter<AddFluentValidationRules>(); 
               
        public async Task<IActionResult> Get([FromQuery]ODataOptions<MenuWithStatusDTO> options)
        {
            var queryDefinition = MenuReadQueries.MenuWithStatusQuery;

            //QueryDefinition meegeven?
            //Dan check in QueryWithODataOptionsAsync of typeof(t) == queryDefinition.DTOType?

            var resultList = await _readQueries.QueryWithODataOptionsAsync(queryDefinition.SqlQuery, options);

            //Todo: geef in een formaat terug, met alle info nodig voor Odata Result etc. 

            return Ok(resultList);
        }

        [Route("v1801")]// jaja, versioning verhaal anders!
        [HttpGet]
        public async Task<IActionResult> Get(ODataOptions<MenuWithStatusDTO_v1801> options)
        {
            var resultList = await _readQueries.QueryWithODataOptionsAsync(MenuReadQueries_v1801.MenuWithStatusQuery.SqlQuery, options);

            //Todo: geef in een formaat terug, met alle info nodig.

            return Ok(resultList);
        }

        [Route("submit")]
        [HttpPost]
        public async Task<IActionResult> SubmitMenu([FromBody]SubmitMenuCommand submitMenuCommand, [FromHeader(Name = "x-requestid")] string requestId)
        {
            try
            {
                _logger.LogInformation("Testje! SubmitMenu.");                
                
                //Todo: Parsing + Identifiedcommand in Extension Method?
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var requestSubmitMenu = new IdentifiedCommand<SubmitMenuCommand, MenuAggr.Menu>(submitMenuCommand, guid);
                    //Todo: Test, Exception indien geen corresponderende handler gevonden ...
                    //--> Iedere Command een test?

                    //Insert Canccelationtoken?
                    var menu = await _mediator.Send(requestSubmitMenu);       
                    return menu == default(MenuAggr.Menu) ? (IActionResult)Ok(menu) : (IActionResult)BadRequest();
                }

                return (IActionResult)BadRequest();
            }
            catch(Exception ex)
            {
                //TODO: Algemene Handler. Lib?
                return (IActionResult)BadRequest(ex);
            }
        }




    }
}
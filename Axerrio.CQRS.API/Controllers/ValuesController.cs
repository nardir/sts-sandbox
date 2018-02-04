using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNetCore.Mvc;
using Moon.OData;
using Moon.OData.Sql;

namespace Axerrio.CQRS.API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ISalesQueries _salesQueries;

        public ValuesController(ISalesQueries salesQueries)
        {
            _salesQueries = salesQueries;
        }

        //http://www.odata.org/getting-started/basic-tutorial/#orderby
        [HttpGet("salesorders")]
        public async Task<IActionResult> GetSalesOrders(ODataOptions<SalesOrder> options)
        {
            //http://localhost:5000/api/values/salesorders?$orderby=orderdate&$top=5&$skip=20
            var validationSettings = new ValidationSettings();

            try
            {
                options.Validate(validationSettings);
            }
            catch (ODataException ex)
            {

            }

            //var orderBy = OrderByClause.Build(options);
            var orderBy = OrderByClause.Build(options, p => "o.[" + p.Name + "]");

            var offsetClause = new OffsetClause(options);
            var offset = offsetClause.Build();

            var selectClause = new SelectClause(options);
            selectClause.ResolveColumn = p => "o.[" + p.Name + "]";
            var select = selectClause.Build();

            var salesOrders = await _salesQueries.SalesOrders();

            var pagedResult = new Paged<SalesOrder>(salesOrders, 5);

            //return Ok(salesOrders);
            return Ok(pagedResult);
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

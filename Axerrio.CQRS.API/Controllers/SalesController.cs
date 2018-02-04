using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Controllers
{
    public class SalesController: ODataController
    {
        private List<SalesOrder> _salesOrders;

        public SalesController()
        {
            _salesOrders = new List<SalesOrder>();

            _salesOrders.Add(new SalesOrder { OrderId = 1, CustomerId = 100, OrderDate = DateTime.Now });
            _salesOrders.Add(new SalesOrder { OrderId = 2, CustomerId = 200, OrderDate = DateTime.Now });
            _salesOrders.Add(new SalesOrder { OrderId = 3, CustomerId = 300, OrderDate = DateTime.Now });
        }

        [HttpGet("odata/salesorders")]
        public IActionResult GetSalesOrders(ODataQueryOptions<SalesOrder> options)
        {
            var salesOrders = options.ApplyTo(_salesOrders.AsQueryable());

            return Ok(salesOrders);
        }
    }
}

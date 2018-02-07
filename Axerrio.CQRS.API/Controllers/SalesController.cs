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
    //http://www.odata.org/blog/trippin-new-odata-v4-sample-service/
    //http://www.odata.org/documentation/odata-version-2-0/uri-conventions/
    //http://www.odata.org/documentation/odata-version-3-0/url-conventions/
    //https://help.nintex.com/en-us/insight/OData/HE_CON_ODATAQueryCheatSheet.htm


    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/People?$filter=contains(FirstName,%20%27Sco%27)
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/People('russellwhyte')/Friends?$select=FirstName
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/People?$select=FirstName,Friends&$expand=Friends
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/People?$select=FirstName&$expand=Friends($select=FirstName)
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/Airports?$orderby=Location/Address
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/Airports('KLAX')/Location/Address
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/Airports('KLAX')/Location/Address/$value
    //http://services.odata.org/V4/(S(kmsvgtl11oefkmu4x1ru2bmj))/TripPinServiceRW/People?$select=LastName&$expand=Friends($select=FirstName)

    public class SalesController: ODataController
    {
        private List<SalesOrder> _salesOrders;
        private readonly WorldWideImportersContext _context;

        public SalesController(WorldWideImportersContext context)
        {
            _context = context;

            _salesOrders = new List<SalesOrder>();

            _salesOrders.Add(new SalesOrder { OrderID = 1, CustomerID = 100, OrderDate = DateTime.Now });
            _salesOrders.Add(new SalesOrder { OrderID = 2, CustomerID = 200, OrderDate = DateTime.Now });
            _salesOrders.Add(new SalesOrder { OrderID = 3, CustomerID = 300, OrderDate = DateTime.Now });
        }

        [HttpGet("odata/salesorders")]
        public IActionResult GetSalesOrders(ODataQueryOptions<SalesOrder> options)
        {
            var salesOrders = options.ApplyTo(_salesOrders.AsQueryable());
            
            return Ok(salesOrders);
        }

        [HttpGet("odata/salesorders/{id}")]
        public IActionResult GetSalesOrder(int Id, ODataQueryOptions<SalesOrder> options)
        {
            var salesOrder = _salesOrders.Where(so => so.OrderID == Id).SingleOrDefault();
            //var salesOrder = options.ApplyTo(_salesOrders.Where(so => so.OrderId == Id).AsQueryable());

            var result = options.SelectExpand.ApplyTo(salesOrder, new ODataQuerySettings());

            return Ok(result);
        }

        [HttpGet("odata/customers")]
        [EnableQuery()]
        public List<Customer> GetCustomers()
        {
            var customers = _context.Customers.ToList();

            return customers;
        }
    }
}

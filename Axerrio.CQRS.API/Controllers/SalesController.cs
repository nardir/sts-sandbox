using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public class SalesController: Controller //ODataController
    {
        private List<SalesOrder> _salesOrders;
        private readonly WorldWideImportersContext _context;
        private readonly ISalesQueries _salesQueries;

        public SalesController(WorldWideImportersContext context, ISalesQueries salesQueries)
        {
            _context = context;
            _salesQueries = salesQueries;

            _salesOrders = new List<SalesOrder>();

            _salesOrders.Add(new SalesOrder { OrderID = 1, CustomerID = 100, OrderDate = DateTime.Now });
            _salesOrders.Add(new SalesOrder { OrderID = 2, CustomerID = 200, OrderDate = DateTime.Now });
            _salesOrders.Add(new SalesOrder { OrderID = 3, CustomerID = 300, OrderDate = DateTime.Now });
        }

        //http://localhost:5000/odata/salesorders?$expand=Customer($filter=Name eq 'Anna van Zant')&$top=10
        //http://localhost:5000/odata/salesorders?$select=OrderID,Customer&$expand=Customer($select=Name)&$top=10
        //http://localhost:5000/odata/salesorders?$select=OrderID,Customer&$expand=Customer&$top=10&$skip=200&$orderby=OrderID+desc

        [HttpGet("odata/salesorders")]
        public async Task<IActionResult> GetSalesOrders(ODataQueryOptions<SalesOrder> options)
        {
            //var salesOrders = options.ApplyTo(_salesOrders.AsQueryable());
            //var salesOrders = await options.ApplyTo(_context.SalesOrders).Cast<dynamic>().ToListAsync();
            var salesOrdersQuery = options.ApplyTo(_context.SalesOrders).Cast<dynamic>();
            var salesOrders = await salesOrdersQuery.ToListAsync();

            return Ok(salesOrders);
        }

        [HttpGet("odata/salesorders2")]
        [EnableQuery]
        public IEnumerable<SalesOrder> GetSalesOrders2()
        {
            return _context.SalesOrders;
        }

        [HttpGet("odata/salesorders/{id}")]
        public IActionResult GetSalesOrder(int Id, ODataQueryOptions<SalesOrder> options)
        {
            var salesOrder = _context.SalesOrders.Where(so => so.OrderID == Id).SingleOrDefault();
            //var salesOrder = options.ApplyTo(_salesOrders.Where(so => so.OrderId == Id).AsQueryable());

            if (salesOrder == null)
                return NotFound();

            //var result = (dynamic) options.SelectExpand.ApplyTo(salesOrder, new ODataQuerySettings());
            var result = options.SelectExpand.ApplyTo(salesOrder, new ODataQuerySettings());

            return Ok(result);
        }

        [HttpGet("odata/customers")]
        [EnableQuery()]
        public IEnumerable<Customer> GetCustomers()
        {
            var customers = _context.Customers;

            return customers;
        }

        [HttpGet("odata/customers2")]
        public async Task<IActionResult> GetCustomers2(ODataQueryOptions<Customer> options)
        {
            //var customers =  options.ApplyTo(_context.Customers).Cast<dynamic>().ToList();
            //var customers = await options.ApplyTo(_context.Customers).Cast<dynamic>().ToListAsync();
            var customersQuery = options.ApplyTo(_context.Customers).Cast<dynamic>();
            var customers = await customersQuery.ToListAsync(); //Hier wordt de query uitgevoerd

            return Ok(customers);
        }

        [HttpGet("odata/salesorders3")]
        public async Task<IActionResult> GetSalesOrders3()
        {
            var salesOrders = await _salesQueries.SalesOrdersAsync();

            return Ok(salesOrders);
            
        }
    }
}

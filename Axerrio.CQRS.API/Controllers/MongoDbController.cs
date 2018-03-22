using Axerrio.CQRS.API.Application.MongoDB;
using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Controllers
{
    //https://blog.oz-code.com/how-to-mongodb-in-c-part-1/
    //https://blog.oz-code.com/how-to-mongodb-in-c-part-2/
    //
    //http://www.qappdesign.com/using-mongodb-with-net-core-webapi/

    //https://www.codementor.io/pmbanugo/working-with-mongodb-in-net-1-basics-g4frivcvz
    //https://www.codementor.io/pmbanugo/working-with-mongodb-in-net-2-retrieving-mrlbeanm5
    //https://www.codementor.io/pmbanugo/working-with-mongodb-in-net-part-3-skip-sort-limit-and-projections-oqfwncyka

    public class MongoDbController: Controller
    {
        private readonly SalesMongoDbContext _context;
        private readonly WorldWideImportersContext _dbContext;

        public MongoDbController(SalesMongoDbContext context, WorldWideImportersContext dbContext)
        {
            _context = context;
            _dbContext = dbContext;
        }

        [HttpPost("mongo/orders/{id:int}")]
        public async Task<IActionResult> MoveSalesOrderToMongoDb(int id)
        {
            var query = _dbContext.SalesOrders
                .Include(o => o.Customer)
                .ThenInclude(c => c.CustomerCategory)
                .Include(o => o.SalesOrderLines)
                .Where(o => o.OrderID == id);

            var order = await query.SingleOrDefaultAsync();

            if (order != null)
                await _context.SalesOrders.InsertOneAsync(order);

            return Ok();
        }

        [HttpPost("mongo/orders/customers/{customerId}")]
        public async Task<IActionResult> MoveSalesOrdersToMongoDb(int customerId)
        {
            var query = _dbContext.SalesOrders
                .Include(o => o.Customer)
                .ThenInclude(c => c.CustomerCategory)
                .Include(o => o.SalesOrderLines)
                .Where(o => o.CustomerID == customerId);

            var orders = await query.ToListAsync();

            await _context.SalesOrders.InsertManyAsync(orders);

            return Ok();
        }

        [HttpGet("mongo/orders/{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var docFilter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var collection = _context.Database.GetCollection<BsonDocument>("Orders");

            //var cursor = await collection.FindAsync<BsonDocument>(docFilter);
            //await cursor.MoveNextAsync();
            //IEnumerable<BsonDocument> batch = cursor.Current;
            //var doc = batch.FirstOrDefault();
            //var byteCount = doc.ToBson().Length;

            var doc2 = await collection.FindSync(docFilter).FirstOrDefaultAsync();
            var byteCount2 = doc2.ToBson().Length;

            var filter = Builders<SalesOrder>.Filter.Where(o => o.OrderID == id);

            var order = await _context.SalesOrders.Find(filter).SingleOrDefaultAsync();

            return Ok(order);
        }


        [HttpGet("mongo/customers")]
        public IActionResult GetCustomers()
        {
            return Ok();
        }

        [HttpPost("mongo/customers/{id:int}")]
        public async Task<IActionResult> MoveCustomerToMongoDb(int id)
        {
            var query = _dbContext.Customers
                .Where(c => c.CustomerID == id)
                .Include(c => c.CustomerCategory);

            var customer = await query.SingleOrDefaultAsync();

            try
            {
                await _context.Customers.InsertOneAsync(customer);
            }
            catch (Exception ex)
            {

            }

            return Ok(customer);
        }

        [HttpGet("mongo/customers/{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var filter = Builders<Customer>.Filter.Eq("CustomerID", id);

            var customer = await _context.Customers
                            .Find(filter)
                            .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost("mongo/customers/{search}")]
        public async Task<IActionResult> MoveCustomersToMongoDb(string search)
        {
            var query = _dbContext.Customers
                .Where(c => EF.Functions.Like(c.Name, string.Format($"%{search}%")))
                .Include(c => c.CustomerCategory);

            var customers = await query.ToListAsync();

            try
            {
                await _context.Customers.InsertManyAsync(customers);
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }

        [HttpDelete("mongo/customers/{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var filter = Builders<Customer>.Filter.Eq("CustomerID", id);

            await _context.Customers.DeleteOneAsync(filter);

            return NoContent();
        }

        [HttpDelete("mongo/customers")]
        public async Task<IActionResult> DeleteCustomers()
        {
            var filter = Builders<Customer>.Filter.Empty;

            //var result = await _context.Customers.DeleteManyAsync(new BsonDocument());
            var result = await _context.Customers.DeleteManyAsync(filter);

            return NoContent();
        }
    }
}

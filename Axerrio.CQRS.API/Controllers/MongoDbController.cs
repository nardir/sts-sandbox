using Axerrio.CQRS.API.Application.MongoDB;
using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Controllers
{
    //https://blog.oz-code.com/how-to-mongodb-in-c-part-1/

    public class MongoDbController: Controller
    {
        private readonly SalesMongoDbContext _context;
        private readonly WorldWideImportersContext _dbContext;

        public MongoDbController(SalesMongoDbContext context, WorldWideImportersContext dbContext)
        {
            _context = context;
            _dbContext = dbContext;
        }

        [HttpGet("mongo/customers")]
        public IActionResult GetCustomers()
        {
            return Ok();
        }

        [HttpPost("mongo/customers/{id}")]
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

        [HttpGet("mongo/customers/{id}")]
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
    }
}

using Axerrio.BB.DDD.Infrastructure.Query;
using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Extensions;
using Axerrio.BB.DDD.Query.API.Data;
using Axerrio.BB.DDD.Query.API.Model;
using Axerrio.BB.DDD.Query.API.Parser;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Controllers
{
    public class CustomerController: Controller
    {
        private readonly IQueryService _queryService;

        public CustomerController(IQueryService queryService)
        {
            EnsureArg.IsNotNull(queryService, nameof(queryService));

            _queryService = queryService;
        }

        [HttpGet("api/customers")]
        public async Task<IActionResult> GetCustomers(Specification<Customer> specification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customers = await _queryService.GetCustomersAsync(specification);

            return Ok(customers);
        }

        [HttpGet("api/customersbylike/{search}")]
        public async Task<IActionResult> GetCustomersByLike(string search)
        {
            var specification = new Specification<Customer>();

            specification.Where("@like(Name, @p0)", search);

            var customers = await _queryService.GetCustomersAsync(specification);

            return Ok(customers);
        }

        [HttpGet("parseordering")]
        public IActionResult ParseOrdering()
        {
            ////Expression<Func<Customer, object>> expression = (Customer c) => c.AccountOpenedDate;
            //Expression<Func<Customer, object>> expression = (Customer c) => "Testname";

            //LambdaExpression lambda = expression;

            var orderingText = "Customer.Name, AccountOpenedDate desc";

            //var tokenDefinitions = new List<TokenDefinition>()
            //{
            //    new TokenDefinition(OrderingTokenType.Comma, ",", 1),
            //    new TokenDefinition(OrderingTokenType.Direction, "asc|ascending|desc|descending", 1),
            //    new TokenDefinition(OrderingTokenType.Property, @"[\p{L}\d\.]+", 2),
            //};

            //var tokenizer = new PrecedenceBasedRegexTokenizer(tokenDefinitions);

            //var tokens = tokenizer.Tokenize(orderingText).ToList();

            var ordering = new Ordering();

            //var parser = new OrderingParser();

            //parser.Parse(ordering, tokens);

            //var parser = new OrderingParser();

            //parser.Parse(ordering, orderingText);

            OrderingParser.Parse(ordering, orderingText);


            return Ok();
        }
    }
}
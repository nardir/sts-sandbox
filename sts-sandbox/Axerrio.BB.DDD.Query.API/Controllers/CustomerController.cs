using Axerrio.BB.DDD.Infrastructure.Query;
using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Helpers;
using Axerrio.BB.DDD.Infrastructure.Query.ModelBinder;
using Axerrio.BB.DDD.Infrastructure.Query.Sql;
using Axerrio.BB.DDD.Query.API.Data;
using Axerrio.BB.DDD.Query.API.Model;
using EnsureThat;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Axerrio.BB.DDD.Infrastructure.Query.ModelBinder.OrderingParser;

namespace Axerrio.BB.DDD.Query.API.Controllers
{
    public class CustomerController : Controller
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

            var orderByBuilder = new OrderBySqlBuilder(specification)
                .WithResolveColumn(mi =>
                {
                    var alias = "c";

                    if (mi.ReflectedType == typeof(CustomerCategory))
                        alias = "cat";

                    return $"{alias}.{mi.Name}";
                });

            var orderBy = orderByBuilder.Build();

            var customers = await _queryService.GetCustomersAsync(specification);

            return Ok(customers);
        }

        [HttpGet("api/pagedcustomers")]
        public async Task<IActionResult> GetPagedCustomers(Specification<Customer> specification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pagedCustomers = await _queryService.GetPagedCustomersAsync(specification);

            return Ok(pagedCustomers);
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

            var ordering = new Orderings();

            OrderingParser.Parse(ordering, orderingText);

            return Ok();
        }

        [HttpGet("properties/{name}")]
        public IActionResult GetPropertyName(string name)
        {
            try
            {
                var expression = DynamicExpressionParser.ParseLambda(typeof(Customer), null, name);

                var memberName = MemberExtractor.Extract(expression);
            }
            catch (Exception ex)
            {

            }

            Expression<Func<Customer, object>> expression2 = (Customer c) => c.AccountOpenedDate;

            var memberName2 = MemberExtractor.Extract(expression2);

            ISpecification<Customer> specification = new Specification<Customer>();

            specification.OrderBy(c => c.AccountOpenedDate, ascending: false)
                .ThenBy("Name");
            
            return Ok();
        }

        [HttpGet("projection")]
        public IActionResult Projection([FromQuery(Name = "$filter")] string filter)
        {
            try
            {
                var p1 = DynamicExpressionParser.ParseLambda(typeof(Customer), null, $"new ({filter})");
            }
            catch (Exception ex)
            {

            }


            return Ok();
        }
    }
}
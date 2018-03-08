using Axerrio.CQRS.API.Application.Query;
using LinqKit;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Controllers
{
    public class CustomerController: Controller
    {
        WorldWideImportersContext _context;
        WorldWideImportersQueryContext _queryContext;

        public CustomerController(WorldWideImportersContext context, WorldWideImportersQueryContext queryContext)
        {
            _context = context;
            _queryContext = queryContext;
        }

        [HttpGet("customers")]
        public IActionResult GetCustomers()
        {
            decimal creditLimit = 1_700;

            var query = _context.Customers.Where(c => c.CreditLimit == creditLimit)
                .Select(c => new
                {
                    c.Name,
                    c.AccountOpenedDate
                }
                )
                .Cast<dynamic>();

            //var query = _context.Customers.Where(c => c.CreditLimit == 1_700).Select(c => c.Name);

            (SelectExpression DatabaseExpression, IReadOnlyDictionary<string, object> Parameters) compilation =
                _context.Compile(query.Expression);

            var command = _context.Generate(compilation.DatabaseExpression, compilation.Parameters);

            dynamic customers = query.ToList();

            return Ok(customers);
        }

        [HttpGet("webcustomers")]
        public IActionResult GetWebCustomers(ODataQueryOptions<WebCustomer> options)
        {
            IQueryable<WebCustomer> baseQuery = Enumerable.Empty<WebCustomer>().AsQueryable();
            var provider = new ODataQueryProvider();
            IQueryable<WebCustomer> baseQuery2 = new Query<WebCustomer>(provider);

            #region filter
            var f = options.Filter.ApplyTo(baseQuery2, new ODataQuerySettings());

            var baseQuery3 = new QuerySpecification<WebCustomer>();
            var f3 = options.Filter.ApplyTo(baseQuery3, new ODataQuerySettings());

            //var filterQueryable = options.Filter.ApplyTo(baseQuery, new ODataQuerySettings());

            //MethodCallExpression filterExpression = (MethodCallExpression)filterQueryable.Expression;
            //UnaryExpression filterQuote = (UnaryExpression)filterExpression.Arguments[1];
            //LambdaExpression filterLambda = (LambdaExpression)filterQuote.Operand;
            //Expression<Func<WebCustomer, bool>> filterPredicate = (Expression<Func<WebCustomer, bool>>)filterLambda;
            #endregion

            //var filter = options.ApplyTo(specification);

            #region selector
            //IQueryable<WebCustomer> selectQueryable = Enumerable.Empty<WebCustomer>().AsQueryable();
            ////var selectorQueryable = options.SelectExpand.ApplyTo(selectQueryable, new ODataQuerySettings());
            var selectorQueryable = options.SelectExpand.ApplyTo(baseQuery, new ODataQuerySettings());
            MethodCallExpression selectorExpression = (MethodCallExpression)selectorQueryable.Expression;
            //UnaryExpression selectorQuote = (UnaryExpression)selectorExpression.Arguments[1];
            //LambdaExpression selectorLambda = (LambdaExpression)selectorQuote.Operand;

            //var s = options.SelectExpand.ApplyTo(baseQuery2, new ODataQuerySettings());
            #endregion




            //var funcType = typeof(Func<,>).MakeGenericType(typeof(WebCustomer), selectorLambda.Type);
            //var expType = typeof(Expression<>).MakeGenericType(funcType);

            //bool allSelected = options.SelectExpand.SelectExpandClause.AllSelected;
            //var items = options.SelectExpand.SelectExpandClause.SelectedItems;

            //var typedExpression = (Expression<Func<Customer, object>>)
            //          Expression.Lambda(funcType, selectorLambda.Body, selectorLambda.Parameters);


            //specification = specification.Where(wc => wc.CustomerID > 20 && wc.CustomerID < 100);

            //var predicate = PredicateBuilder.New<WebCustomer>(wc => wc.CustomerID > 20 && wc.CustomerID < 100);

            //(SelectExpression DatabaseExpression, IReadOnlyDictionary<string, object> Parameters) compilation =
            //    _queryContext.Compile(filter.Expression);


            #region Apply specification
            //var queryableSelectMethod = GenericMethodOf(_ => Queryable.Select<int, int>(default(IQueryable<int>), i => i));
            //MethodInfo selectMethod = queryableSelectMethod.MakeGenericMethod(typeof(WebCustomer), selectorLambda.Body.Type);

            IQueryable<WebCustomer> query = _queryContext.WebCustomers;

            //query = query.Where(filterPredicate);

            //var finalQuery = selectMethod.Invoke(null, new object[] { query, selectorLambda }) as IQueryable;
            //var customers = finalQuery.Cast<dynamic>().ToList();
            #endregion


            //var query2 = options.SelectExpand.ApplyTo(query, new ODataQuerySettings()).Cast<dynamic>();


            //PageResult<WebCustomer> pageResult = new PageResult<WebCustomer>()

            #region Order
            //var o = options.OrderBy.ApplyTo(baseQuery2);

            //var orderedQueryable = options.OrderBy.ApplyTo(query);

            //var customers = orderedQueryable.ToList();
            #endregion

            var filterMethodCall = provider.Filter;
            //MethodCallExpression filterExpression = (MethodCallExpression)filterQueryable.Expression;
            UnaryExpression filterQuote = (UnaryExpression)filterMethodCall.Arguments[1];
            //LambdaExpression filterLambda = (LambdaExpression)filterQuote.Operand;

            var extractor = new LambdaExtractor();
            LambdaExpression filterLambda = extractor.Extract(filterMethodCall);

            //Expression<Func<WebCustomer, bool>> filterPredicate = (Expression<Func<WebCustomer, bool>>)filterLambda;
            //var customers = filterMethodCall.Method.Invoke(null, new object[] { query, filterLambda }) as IQueryable;
            var customers = filterMethodCall.Method.Invoke(null, new object[] { query, filterLambda }) as IQueryable;

            LambdaExpression selectorLambda = extractor.Extract(selectorExpression);
            customers = selectorExpression.Method.Invoke(null, new object[] { customers, selectorLambda }) as IQueryable;

            return Ok(customers);
        }

        [HttpGet("testserdes")]
        public IActionResult TestSerDes()
        {
            Expression<Func<WebCustomer, bool>> lambda = c => c.CustomerName.StartsWith("Tail");

            var serializedLambda = JsonNetAdapter.Serialize(lambda);

            var deserializedLambda = JsonNetAdapter.Deserialize<Expression<Func<WebCustomer, bool>>>(serializedLambda);

            var customers = _queryContext.WebCustomers.Where(deserializedLambda)
                .Select(c => c.CustomerName)
                .ToList();

            
            return Ok(customers);
        }

        private static MethodInfo GenericMethodOf(Expression expression)
        {
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            //Contract.Assert(expression.NodeType == ExpressionType.Lambda);
            //Contract.Assert(lambdaExpression != null);
            //Contract.Assert(lambdaExpression.Body.NodeType == ExpressionType.Call);

            return (lambdaExpression.Body as MethodCallExpression).Method.GetGenericMethodDefinition();
        }

        static MethodInfo GenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            return GenericMethodOf(expression as Expression);
        }
    }
}

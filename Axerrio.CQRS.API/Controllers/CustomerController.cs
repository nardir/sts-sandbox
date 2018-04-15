using Axerrio.CQRS.API.Application.Query;
using Axerrio.CQRS.API.Application.Specification;
using LinqKit;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Text.Encodings.Web.Utf8;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using Axerrio.CQRS.API.Application.Filters;
using Newtonsoft.Json;

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

        [HttpGet("testfilter3")]
        public IActionResult TestFilter3()
        {

            IQueryable<Customer> query = _context.Customers;

            bool isOrdered = (query is IOrderedQueryable<Customer>);

            IOrderedQueryable<Customer> result = query.OrderBy(c => c.CustomerID);

            isOrdered = (query is IOrderedQueryable<Customer>);

            //query = query.OrderBy(c => c.Name);
            result = result.ThenBy(c => c.Name);

            //var customers = query.ToList();
            var customers = result.ToList();

            return Ok(customers);
        }

        [HttpGet("testfilter2")]
        public IActionResult TestFilter2()
        {
            var filterSource = new Filter(typeof(Customer)) { Key = 100, Name = "Filter Test" };

            var filterRow = filterSource.AddFilterRow();
            filterSource.AddFilterCondition(filterRow, "CustomerID == 22");

            var filterDef = new FilterDefinition(filterSource);

            var filterTarget = filterDef.Filter;

            var spec = new Specification<Customer>();

            //spec.SetPredicate(filterTarget.Predicate);

            //IQueryable<Customer> query = _context.Customers;

            ////query = query.Where(filterTarget.Predicate).Cast<Customer>();
            //query = query.Where(spec.Predicate);

            //var customers = query.ToList();

            //return Ok(customers);

            return Ok();
        }

        [HttpGet("testfilter")]
        public IActionResult TestFilter()
        {
            IFilter filterSource = new Filter<Customer> { Key = 100, Name = "Filter Test" };

            filterSource.Where("CustomerID == 22");

            string source = JsonConvert.SerializeObject(filterSource);

            ////////////////

            var customerTypeName = typeof(Customer).AssemblyQualifiedName;
            var customerType = Type.GetType(customerTypeName);

            var genericFilterType = typeof(Filter<>);
            var closedFilterType = genericFilterType.MakeGenericType(customerType);

            //IFilter filter = (IFilter) Activator.CreateInstance(closedFilterType);

            //IFilter filterTarget = (IFilter) JsonConvert.DeserializeObject(source, closedFilterType);
            Filter<Customer> filterTarget = (Filter<Customer>)JsonConvert.DeserializeObject(source, closedFilterType);

            /////
            var spec = new Specification<Customer>();

            spec.SetPredicate(filterTarget.Predicate);

            IQueryable<Customer> query = _context.Customers;

            //query = query.Where(filterTarget.Predicate).Cast<Customer>();
            query = query.Where(spec.Predicate);

            var customers = query.ToList();

            return Ok(customers);
        }

        [HttpGet("customersbyspec")]
        public IActionResult GetCustomerBySpecification(Specification<Customer> specification)
        {
            
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }

        [HttpGet("efmodel")]
        public IActionResult GetEFModel()
        {
            var model = _context.Model;

            var custType = model.FindEntityType("Axerrio.CQRS.API.Application.Query.Customer");
            if (custType == null)
                custType = model.GetEntityTypes().Where(et => et.Name == "").SingleOrDefault();

            var props = custType.GetProperties().ToArray();
            var navProps = custType.GetNavigations();
            var annotations = custType.GetAnnotations();

            var propNames = props.Select(p => p.Name);

            var property = props.Where(p => p.Name == "Name").SingleOrDefault();
            if (property != null)
            {
                var propAnn = property.GetAnnotations();
            }
        
            return Ok(propNames);
        }


        [HttpGet("orders4")]
        public IActionResult GetOrders4([FromQuery] int page, [FromQuery] int size)
        {
            //IQueryable<SalesOrderLine> baseQuery = _context.SalesOrderLines;

            //var groupQuery = baseQuery
            //    .GroupBy(l => l.OrderID)
            //    .Select(g => new { OrderID = g.Key, TotalAmount = g.Sum(l => l.Quantity * l.UnitPrice) });

            IQueryable<SalesOrder> orderQuery = _context.SalesOrders;
            IQueryable<SalesOrderLine> orderLineQuery = _context.SalesOrderLines;

            IQueryable<Customer> customerQuery = _context.Customers;
            IQueryable<CustomerCategory> customerCategoryQuery = _context.CustomerCategories;

            var query3 = orderLineQuery
                .GroupBy(l => new { l.OrderID })
                .Select(g => new { g.Key.OrderID, TotalAmount = g.Sum(l => l.Quantity * l.UnitPrice) });

            //query3 = query3.Where(g => g.TotalAmount < 1000); //Evaluated locally and strips the group by from the query

            var result3 = query3.ToList();
            result3 = result3.Where(g => g.TotalAmount < 1000).ToList();

            int t = 4;

            //var query2 = customerCategoryQuery
            //    .Select(cc => new
            //    {
            //        Category = cc,
            //        Customers = customerQuery.Where(c => c.CustomerCategoryID == cc.CustomerCategoryID)
            //                                .DefaultIfEmpty() //Changes the query to include categories that have no customers
            //    })
            //    .SelectMany(collectionSelector: category => category.Customers,
            //                resultSelector: (category, customers) => new
            //                {
            //                    category.Category.CustomerCategoryName,
            //                    customers.Name
            //                });

            //var query2 = customerCategoryQuery
            //    .SelectMany(collectionSelector: category => customerQuery.Where(c => c.CustomerCategoryID == category.CustomerCategoryID).DefaultIfEmpty(),
            //                resultSelector: (category, customer) => new { category.CustomerCategoryName, customer.Name });

            //var query2 = customerCategoryQuery
            //    .GroupJoin
            //    (
            //        inner: customerQuery,
            //        outerKeySelector: cc => cc.CustomerCategoryID,
            //        innerKeySelector: c => c.CustomerCategoryID,
            //        resultSelector: (cc, cs) => new
            //        {
            //            Category = cc,
            //            Customers = cs
            //            //CustomerName = cs.Select(c => c.Name)
            //        }
            //    ).SelectMany
            //    (
            //        collectionSelector: g => g.Customers, //INNER JOIN
            //        //collectionSelector: g => g.Customers.DefaultIfEmpty(), //LEFT OUTER JOIN 
            //        resultSelector: (g, c) => new { Category = g.Category.CustomerCategoryName, Customer = c.Name }
            //    );

            //var query2 = customerQuery
            //    .Select(c => new { c.Name, CreditLimitSet = (c.CreditLimit != null) });

            //var query2 = customerQuery.Select(c => new Customer { CustomerID = c.CustomerID, Name = c.Name });

            //var query2 = customerQuery
            //    .Select(c => new { c.CustomerID, Category = customerCategoryQuery.Where(cc => cc.CustomerCategoryID == c.CustomerCategoryID).Select(cc => cc.CustomerCategoryName) });

            //var query2 = customerQuery.Select(c => new { c.CustomerID, c.CustomerCategory.CustomerCategoryName });
            var query2 = customerQuery.Select(c => new { c.CustomerID, c.CustomerCategory });

            //var query2 = customerQuery
            //    .Join
            //    (
            //        inner: customerCategoryQuery,
            //        outerKeySelector: c => c.CustomerCategoryID,
            //        innerKeySelector: cc => cc.CustomerCategoryID,
            //        //resultSelector: (c, cc) => new { c.CustomerID, cc.CustomerCategoryName }
            //        resultSelector: (c, cc) => new { c.CustomerID, Category = cc }
            //    );

            var query2Results = query2.ToList();

            int i = 4;

            //var customerID = -1;
            //customerID = 22;

            //customerQuery = customerQuery
            //    .Where(c => c.CustomerID == customerID)
            //    .DefaultIfEmpty();

            //var customers = customerQuery.ToList();

            //var groupedCustomersQuery = customerQuery
            //    .GroupBy(keySelector: c => new { c.CustomerCategoryID },
            //             resultSelector: (key, group) => new { key.CustomerCategoryID, Count = group.Count() }).Cast<dynamic>();

            //var groupedCustomers = groupedCustomersQuery.ToList();

            //var groupQuery = orderLineQuery
            //    .Where(l => l.SalesOrder.CustomerID == 507)
            //    //.OrderBy(l => l.SalesOrder.OrderDate)
            //    //.Skip(10)
            //    //.Take(5)
            //    .GroupBy(l => new { l.OrderID })
            //    .Select(g => new
            //    {
            //        g.Key.OrderID,
            //        TotalAmount = g.Sum(l => l.Quantity * l.UnitPrice),
            //        TotalQuantity = g.Sum(l => l.Quantity),
            //        AvgUnitprice = g.Average(l => l.UnitPrice)
            //    });
            //    //.OrderBy(a => a.OrderID).Skip(10).Take(5);

            //var groupQuery = orderQuery
            //    .Where(o => o.CustomerID == 507)
            //    .OrderByDescending(o => o.OrderDate)
            //    .Skip(10)
            //    .Take(5)
            //    .Select(o => new
            //        {
            //            o.OrderID,
            //            o.OrderDate,
            //            CustomerName = o.Customer.Name
            //        }
            //    );

            //var groupQuery = orderQuery
            //    .Where(o => o.CustomerID == 507)
            //    .OrderByDescending(o => o.OrderDate)
            //    .Skip(10)
            //    .Take(5)
            //    .Join
            //    (
            //        orderLineQuery
            //        , o => o.OrderID
            //        , l => l.OrderID
            //        , (o, l) => new
            //        {
            //            o.OrderID,
            //            o.OrderDate,
            //            CustomerName = o.Customer.Name,
            //            //l.Description,
            //            //l.Quantity,
            //            //l.UnitPrice,
            //            TotalAmount = o.SalesOrderLines.Sum(sl => sl.Quantity * sl.UnitPrice),
            //            TotalQuantity = o.SalesOrderLines.Sum(sl => sl.Quantity),
            //        }
            //    );

            //var customerID = 507;

            orderQuery = orderQuery.Where(o => EF.Functions.Like(o.Customer.Name, "Tail%"));

            var total = orderQuery.Count();
            var take = 20;
            var skip = page * take;

            //EF.Functions.DateDiffDay()

            var groupQuery = orderQuery
                //.Where(o => o.CustomerID == customerID)
                //.Where(o => EF.Functions.Like(o.Customer.Name, "Tail%"))
                .OrderByDescending(o => o.OrderDate)
                .Skip(skip)
                .Take(take)
                .Join
                (
                    orderLineQuery
                    , o => o.OrderID
                    , l => l.OrderID
                    , (o, l) => new
                    {
                        o.OrderID,
                        o.OrderDate,
                        CustomerName = o.Customer.Name,
                        l.Quantity,
                        l.UnitPrice
                    }
                )
                .GroupBy(ol => new { ol.OrderID, ol.OrderDate, ol.CustomerName })
                .Select(g => new { g.Key.OrderID, g.Key.OrderDate, g.Key.CustomerName, TotalAmount = g.Sum(ol => ol.Quantity * ol.UnitPrice), TotalQuantity = g.Sum(ol => ol.Quantity), AvgUnitPrice = g.Average(ol => ol.UnitPrice) });


            //var groupQuery = orderQuery.Where(o => o.CustomerID == 507)
            //    .Select(o => new
            //    {
            //        o.OrderID,
            //        //TotalAmount = o.SalesOrderLines.Sum(l => l.Quantity * l.UnitPrice),
            //        //TotalQuantity = o.SalesOrderLines.Sum(l => l.Quantity),
            //        AvgUnitPrice = o.SalesOrderLines.Average(l => l.UnitPrice) //Dit veroozaakt een N + 1 probleem
            //    });

            var orders = groupQuery.ToList();

            //var result = groupQuery.PageResult((skip / take) + 1, take);

            return Ok(orders);
        }

        [HttpGet("orders3")]
        public IActionResult GetOrders3()
        {
            //var lambda = DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "CustomerID == 507");
            //Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>) DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "CustomerID == @0", 507);
            //Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>)DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "Customer.Name.Contains(@0)", "Taj");
            //Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>)DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "Customer.Name.StartsWith(@0)", "Tail");

            //https://dotnetcoretutorials.com/2016/12/31/using-url-encode-net-core/
            //https://stackoverflow.com/questions/44920875/url-encode-and-decode-in-asp-net-core
            var queryString = "$filter=CustomerID == 507&$select=CustomerID, CustomerName";
            var urlEncoded = WebUtility.UrlEncode(queryString);
            var decoded = WebUtility.UrlDecode(urlEncoded);

            //Expression<Func<SalesOrder, string, bool>> likeLambda = (so, s) => EF.Functions.Like(so.Customer.Name, s);
            Expression<Func<string, string, bool>> likeLambda = (n, s) => EF.Functions.Like(n, s);

           
            var p = new Dictionary<string, object>();
            p.Add("@like", likeLambda);
            //p.Add("@search", "%Taj%");
            p.Add("@search", "Taj");

            //Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>)DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "@0(it, @1)", likeLambda, "Tail%");
            //Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>)DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "@like(Customer.Name, @search)", p);
            Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>)DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "Customer.Name.StartsWith(@search)", p);
            //Expression<Func<SalesOrder, bool>> lambda = (Expression<Func<SalesOrder, bool>>)DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(bool), "Microsoft.EntityFrameworkCore.EF.Functions.Like(o.Customer.Name, @0)", "Tail");

            //Expression<Func<SalesOrder, object>> orderby = (Expression<Func<SalesOrder, object>>) DynamicExpressionParser.ParseLambda(typeof(SalesOrder), typeof(object), "Customer.Name");

            Expression<Func<DateTime, DateTime, int>> dateDiffDay = (sd, ed) => EF.Functions.DateDiffDay(sd, ed);

            //var query = _context.SalesOrders.Where("CustomerID == @0", 507);
            //var query = _context.SalesOrders.Where(lambda).Cast<SalesOrder>();
            //var query = _context.SalesOrders.Where(lambda);
            IQueryable<SalesOrder> query = _context.SalesOrders;
            //query = query.Where(lambda);
            //query = query.Where(o => EF.Functions.Like(o.Customer.Name, "%Taj%"));
            //query = query.Where(o => o.Customer.Name.Contains("Taj"));

            //query = query.Where(o => o.SalesOrderLines.Any(l => l.StockItemID == 212));
            query = query.Where("SalesOrderLines.Any(l => l.StockItemID == @0)", 212);

            //query = query.Where(o => DynamicFunctions.Like(o.Customer.Name, "%Taj"));
            //query = query.Where(o => o.Customer.Name.StartsWith("Tail"));

            //var test = query.Select(o => new { o.OrderID, o.Customer.Name });
            query = query.OrderBy("Customer.Name desc");

            LambdaExpression selector =  DynamicExpressionParser.ParseLambda(typeof(SalesOrder), null , "new ( OrderID, Customer.Name as CustomerName)");
            var selectMethod = ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(typeof(SalesOrder), selector.Body.Type);

            //var projection = query.Select("new ( OrderID, Customer.Name as CustomerName)")
            //    .Cast<dynamic>();
            var projection = selectMethod.Invoke(null, new object[] { query, selector }) as IQueryable;

            var orders = projection.Cast<dynamic>().ToList();

            return Ok(orders);
        }

        [HttpGet("orders2")]
        public IActionResult GetOrders2(ODataQueryOptions<SalesOrder> options)
        {
            //http://localhost:5000/orders2?$select=OrderID,Customer&$expand=Customer($select=Name)
            //http://localhost:5000/orders2?$select=OrderID&$filter=CustomerID+eq+507&$expand=Customer($select=Name)
            //http://localhost:5000/orders2?$filter=contains(Customer/Name, 'Baran')&$select=OrderID,Customer&$expand=Customer
            //http://localhost:5000/orders2?$filter=SalesOrderLines/any()
            //http://localhost:5000/orders2?$filter=SalesOrderLines/any(l:l/Quantity+gt+100)
            //http://localhost:5000/orders2?$filter=OrderID+eq+706&$select=OrderID,SalesOrderLines&$expand=SalesOrderLines
            var specification = new Specification<SalesOrder>();
            var specificationQuery = new ODataQueryable<SalesOrder>(specification);

            options.ToSpecification(specificationQuery);

            //var count = _context.SalesOrders.Where(specification.Predicate).Count();

            var query = _context.SalesOrders.ApplySpecification<SalesOrder, dynamic>(specification);

            var orders = query.ToList();

            return Ok(orders);
        }

        [HttpGet("webcustomerstest")]
        public IActionResult GetWebCustomersTest(ODataQueryOptions<WebCustomer> options)
        {
            //http://www.odata.org/documentation/odata-version-2-0/uri-conventions/

            //Validate options
            var settings = new ODataQuerySettings();
            settings.EnsureStableOrdering = false;

            var specification = new Specification<WebCustomer>();
            var specificationQuery = new ODataQueryable<WebCustomer>(specification);

            options.ToSpecification(specificationQuery, settings);

            
            //if (options.SelectExpand != null)
            //{

            //    var fields = options.SelectExpand.RawSelect.Split(',');

            //    specification.AddSelector(null);
            //    specification.AddSelector<dynamic>(ToDynamic<WebCustomer>(fields.ToHashSet()));
            //}

            ///////////////////////
            if (!specification.HasSelector)
            {
                specification.AddSelector(c => new { c.CustomerName, c.PrimaryContact });
            }
            
            if (!specification.HasTake)
            {
                specification.OrderBy(c => c.CustomerID);
                specification.Take = 10;
                specification.Skip = 100;
            }

            var query = _queryContext.WebCustomers.ApplySpecification<WebCustomer, dynamic>(specification);

            var customers = query.ToList();

            var spec2 = new Specification<WebCustomer>();
            spec2.OrderBy(c => c.CustomerName)
                .AddSelector(c => new { c.CustomerID, c.PrimaryContact });
                //.AddSelector(c => c.CustomerName);

            spec2.Take = 10;
            spec2.Skip = 100;

            var customers2 = _queryContext.WebCustomers.ApplySpecification<WebCustomer, dynamic>(spec2).ToList();



            return Ok(customers);
        }

        [HttpGet("spec2")]
        public IActionResult TestSpec2()
        {
            var p1 = PredicateBuilder.New<WebCustomer>(true);

            var started = p1.IsStarted;
            p1 = p1.Start(c => c.CustomerID == 22);
            started = p1.IsStarted;

            var p2 = PredicateBuilder.New<WebCustomer>(true);
            p2 = p2.And(c => c.CustomerName.Contains("Tail"));
            started = p2.IsStarted;

            return Ok();
        }

        [HttpGet("spec")]
        public IActionResult TestSpec()
        {
            Expression<Func<WebCustomer, bool>> predicate = c => c.CustomerID > 100;

            var specification = new Specification<WebCustomer>(predicate);

            specification.AddSelector(c => new { c.CustomerID, c.CustomerName, c.PrimaryContact });

            var selector = specification.Selector;

            IQueryable<WebCustomer> customerQuery = _queryContext.WebCustomers;

            customerQuery = customerQuery
                .Where(specification.Predicate)
                .OrderBy(c => c.CustomerName)
                .Take(10);

            var selectMethod = ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(typeof(WebCustomer), typeof(string));
            Expression<Func<WebCustomer, string>> selectLambda = c => c.CustomerName;
            var mce = Expression.Call(selectMethod, _queryContext.WebCustomers.AsQueryable().Expression, Expression.Quote(selectLambda));

            var spec2 = new Specification<WebCustomer>()
                .AddSelector(mce);

            var selector2 = spec2.Selector;

            var pq2 = selector2.Method.Invoke(null, new object[] { _queryContext.WebCustomers, selector2.Lambda }) as IQueryable;

            var customers2 = pq2.Cast<dynamic>().ToList();

            var projectedQuery = selector.Method.Invoke(null, new object[] { customerQuery, selector.Lambda }) as IQueryable;

            var customers = projectedQuery.Cast<dynamic>().ToList();

            return Ok(customers);
        }

        [HttpGet("orderlines")]
        public IActionResult GetOrderLines()
        {

            var predicate = PredicateBuilder.New<SalesOrderLine>(false);

            predicate = predicate.And(l => l.StockItemID == 151);
            predicate = predicate.And(l => l.Quantity == 10);

            var linesQuery = _context.SalesOrderLines.Where(predicate);

            var lines = linesQuery.ToList();

            return Ok(lines);
        }

        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            Expression<Func<SalesOrderLine, bool>> predicate = l => l.Quantity == 9;
            Expression<Func<SalesOrder, bool>> orderpredicate = so => so.SalesOrderLines.Any(l => l.Quantity == 100);

            //var ordersQuery = _context.SalesOrders.AsExpandable().Where(o => o.SalesOrderLines.Any(predicate.Compile()));
            var ordersQuery = _context.SalesOrders.Where(orderpredicate);

            var orders = ordersQuery.ToList();

            return Ok(orders);
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

        [HttpGet("webcustomers2")]
        public IActionResult GetWebCustomers2()
        {
            Expression<Func<WebCustomer, bool>> predicate = c => c.CustomerID == 20;

            IQueryable<WebCustomer> query = _queryContext.WebCustomers;

            //var queryableWhereMethod = GenericMethodOf(_ => Queryable.Where<int>(default(IQueryable<int>), default(Expression<Func<int, bool>>)));
            var queryableWhereMethod = ExpressionHelperMethods.QueryableWhereGeneric;

            MethodInfo whereMethod = queryableWhereMethod.MakeGenericMethod(typeof(WebCustomer));

            MethodCallExpression whereCallExpression = Expression.Call(whereMethod, query.Expression, Expression.Quote(predicate));

            /////////////////////////////////////////////


            var customersQuery = whereCallExpression.Method.Invoke(null, new object[] { query, predicate }) as IQueryable<WebCustomer>;

            var customers = customersQuery.ToList();

            return Ok(customers);
        }

        [HttpGet("webcustomers")]
        public IActionResult GetWebCustomers(ODataQueryOptions<WebCustomer> options)
        {
            ODataQuerySettings settings = new ODataQuerySettings();
            

            IQueryable<WebCustomer> baseQuery = Enumerable.Empty<WebCustomer>().AsQueryable();

            var provider = new ODataQueryProvider();
            IQueryable<WebCustomer> baseQuery2 = new Query<WebCustomer>(provider);

            var p = options.Skip.ApplyTo(baseQuery2, new ODataQuerySettings());

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

            //var extractor = new LambdaExtractor();
            LambdaExpression filterLambda = LambdaExtractor.Extract(filterMethodCall);

            //Expression<Func<WebCustomer, bool>> filterPredicate = (Expression<Func<WebCustomer, bool>>)filterLambda;
            //var customers = filterMethodCall.Method.Invoke(null, new object[] { query, filterLambda }) as IQueryable;
            var customers = filterMethodCall.Method.Invoke(null, new object[] { query, filterLambda }) as IQueryable;

            LambdaExpression selectorLambda = LambdaExtractor.Extract(selectorExpression);
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

        public static Expression<Func<T, dynamic>> ToDynamic<T>(ISet<string> fields)
        {
            //https://gist.github.com/volak/20f453de023ff75edeb8
            var pocoType = typeof(T);

            var itemParam = Expression.Parameter(pocoType, "x");
            var members = fields.Select(f => Expression.PropertyOrField(itemParam, f));
            var addMethod = typeof(IDictionary<string, object>).GetMethod(
                        "Add", new Type[] { typeof(string), typeof(object) });


            var elementInits = members.Select(m => Expression.ElementInit(addMethod, Expression.Constant(m.Member.Name), Expression.Convert(m, typeof(Object))));

            var expando = Expression.New(typeof(ExpandoObject));
            var body = Expression.ListInit(expando, elementInits);
            var lambda = Expression.Lambda<Func<T, dynamic>>(Expression.ListInit(expando, elementInits), itemParam);

            return lambda;
        }
    }
}

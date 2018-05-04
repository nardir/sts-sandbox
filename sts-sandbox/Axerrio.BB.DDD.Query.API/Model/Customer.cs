using Linq.PropertyTranslator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Model
{
    public class Customer
    {
        public Customer()
        {
            SalesOrders = new List<SalesOrder>();
        }

        public int CustomerID { get; set; }
        public string Name { get; set; }
        public decimal? CreditLimit { get; set; }
        public DateTime AccountOpenedDate { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomerCategoryID { get; set; }
        public CustomerCategory CustomerCategory { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; }

        #region

        private static readonly CompiledExpressionMap<Customer, int> salesOrderCountExpression =
        DefaultTranslationOf<Customer>.Property(p => p.SalesOrderCount).Is(c => c.SalesOrders.Count());

        public int SalesOrderCount => salesOrderCountExpression.Evaluate(this);

        #endregion
        }
}

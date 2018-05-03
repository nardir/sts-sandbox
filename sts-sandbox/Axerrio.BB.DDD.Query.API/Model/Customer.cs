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
    }
}

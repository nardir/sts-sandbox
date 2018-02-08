using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class SalesOrder
    {
        public SalesOrder()
        {
            //SalesOrderLines = new List<SalesOrderLine>();
        }
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }

        public Customer Customer { get; set; }

        //public ICollection<SalesOrderLine> SalesOrderLines { get; set; }
    }
}

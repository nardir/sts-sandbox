using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class SalesOrderLine
    {
        public int OrderLineID { get; set; }
        public int OrderID { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }

        public SalesOrder SalesOrder { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public decimal? CreditLimit { get; set; }
        public DateTime AccountOpenedDate { get; set; }
        public string PhoneNumber { get; set; }
    }
}
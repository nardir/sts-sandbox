using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    //[Table("Customers", Schema ="Website")]
    public class WebCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string PrimaryContact { get; set; }
        public string WebsiteUrl { get; set; }
    }
}

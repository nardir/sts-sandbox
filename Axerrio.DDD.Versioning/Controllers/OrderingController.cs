using Axerrio.DDD.Versioning.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Versioning.Controllers
{
    [Route("api/[controller]")]
    public class OrderingController: Controller
    {
        [HttpGet()]
        public string Home()
        {
            return "Ordering - Home";
        }

        [Route("order")]
        [HttpGet()]
        public Order Order()
        {
            return new Order(1,"a123", "joskes", 1);
        }
    }
}

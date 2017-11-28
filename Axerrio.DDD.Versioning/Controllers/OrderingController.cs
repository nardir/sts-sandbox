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
    }
}

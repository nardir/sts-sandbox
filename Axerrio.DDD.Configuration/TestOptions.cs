using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration
{
    public class TestOptions
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public string[] Names { get; set; }
    }
}
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class TestHostedServiceFactory
    {
        ILogger<TestHostedService> _logger;
        OrderingDbContext _context;

        public TestHostedServiceFactory(ILogger<TestHostedService> logger, OrderingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public TestHostedService Create()
        {
            return new TestHostedService(_logger, _context);
        }
    }
}

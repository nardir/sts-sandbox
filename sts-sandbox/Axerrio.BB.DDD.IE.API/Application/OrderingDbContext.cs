using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Application
{
    public class OrderingDbContext : DbContext, IIntegrationEventsDbContext
    {
    }
}

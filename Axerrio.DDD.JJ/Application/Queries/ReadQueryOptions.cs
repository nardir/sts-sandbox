using EnsureThat;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Queries
{
    public class ReadQueryOptions
    {
        public string ConnectionString { get; set; }
    }    
}

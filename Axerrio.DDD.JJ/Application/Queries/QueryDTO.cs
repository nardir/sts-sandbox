using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu.Application.Queries
{
    public abstract class QueryDTO    
    {
        public string Sql { get; private set; }
    }
}

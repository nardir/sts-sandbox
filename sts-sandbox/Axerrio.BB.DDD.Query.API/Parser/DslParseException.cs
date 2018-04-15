using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class DslParseException: Exception
    {
        public DslParseException(string message): base(message)
        {

        }
    }
}

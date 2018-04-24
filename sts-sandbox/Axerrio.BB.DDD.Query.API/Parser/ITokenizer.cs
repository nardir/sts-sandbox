using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public interface ITokenizer
    {
        IEnumerable<DslToken> Tokenize(string text, List<TokenDefinition> tokenDefinitions);
    }
}
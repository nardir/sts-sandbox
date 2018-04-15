using Axerrio.BB.DDD.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public interface ITokenType: IEnumeration
    {

    }

    public class TokenType : Enumeration<TokenType>//, ITokenType
    {
        public static TokenType SequenceTerminator = new TokenType(-1, nameof(SequenceTerminator));

        static TokenType()
        {
            _items.AddRange(new[] { SequenceTerminator });
        }

        protected TokenType()
        {
        }

        protected TokenType(int id, string name) : base(id, name)
        {
        }
    }
}

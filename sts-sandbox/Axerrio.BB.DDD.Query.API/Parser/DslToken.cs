using Axerrio.BB.DDD.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class DslToken: ValueObject<DslToken>
    {
        public DslToken(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public DslToken(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public TokenType TokenType { get; private set; }
        public string Value { get; private set; }

        public DslToken Clone()
        {
            return new DslToken(TokenType, Value);
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return TokenType;
            yield return Value;
        }
    }
}

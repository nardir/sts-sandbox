using Axerrio.BB.DDD.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class TokenMatch: ValueObject<TokenMatch>
    {
        public TokenType TokenType { get; private set; }
        public string Value { get; private set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }
        public int Precedence { get; private set; }

        public TokenMatch(TokenType tokenType, string value, int startIndex, int endIndex, int precedence)
        {
            TokenType = tokenType;
            Value = value;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Precedence = precedence;
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return TokenType;
            yield return Value;
            yield return StartIndex;
            yield return EndIndex;
            yield return Precedence;
        }
    }
}

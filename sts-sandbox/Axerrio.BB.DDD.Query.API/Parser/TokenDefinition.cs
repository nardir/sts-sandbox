using Axerrio.BB.DDD.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class TokenDefinition: ValueObject<TokenDefinition>
    {
        public TokenDefinition(TokenType tokenType, string regexPattern, int precedence)
        {
            Regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            TokenType = tokenType;
            Precedence = precedence;
        }

        public Regex Regex { get; private set; }
        public TokenType TokenType { get; private set; }
        public int Precedence { get; private set; }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = Regex.Matches(inputString);
            for (int i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch(TokenType, matches[i].Value, matches[i].Index, matches[i].Index + matches[i].Length, Precedence);
            }
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return Regex;
            yield return TokenType;
            yield return Precedence;
        }
    }
}
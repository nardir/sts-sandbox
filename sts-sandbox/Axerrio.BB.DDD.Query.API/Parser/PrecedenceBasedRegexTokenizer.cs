using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class PrecedenceBasedRegexTokenizer : ITokenizer
    {
        public PrecedenceBasedRegexTokenizer()
        {
        }

        public IEnumerable<DslToken> Tokenize(string text, List<TokenDefinition> tokenDefinitions)
        {
            var tokenMatches = FindTokenMatches(text, tokenDefinitions);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new DslToken(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }

            yield return new DslToken(TokenType.SequenceTerminator);
        }

        private List<TokenMatch> FindTokenMatches(string text, List<TokenDefinition> tokenDefinitions)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(text).ToList());

            return tokenMatches;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class Ordering: List<(string KeySelector, bool Ascending)>
    {

    }

    public class OrderingParser : DslParser<OrderingParser, Ordering, PrecedenceBasedRegexTokenizer>
    {
        public OrderingParser()
        {
        }

        protected override void BuildTokenDefinitions()
        {
            _tokenDefinitions = new List<TokenDefinition>()
            {
                new TokenDefinition(OrderingTokenType.Comma, ",", 1),
                new TokenDefinition(OrderingTokenType.Direction, "asc|ascending|desc|descending", 1),
                new TokenDefinition(OrderingTokenType.Property, @"[\p{L}\d\.]+", 2),
            };
        }

        protected override void ParseDsl()
        {
            ParseOrdering();
        }

        private void ParseOrdering()
        {
            //Setup Order
            (string keySelector, bool ascending) orderBy;

            var propertyToken = ReadToken(OrderingTokenType.Property);
            DiscardToken(OrderingTokenType.Property);

            orderBy.keySelector = propertyToken.Value;
            orderBy.ascending = true; //default

            if (FirstTokenType == OrderingTokenType.Direction)
            {
                var directionToken = ReadToken(OrderingTokenType.Direction);
                DiscardToken(OrderingTokenType.Direction);

                if (directionToken.Value == "desc" || directionToken.Value == "descending")
                    orderBy.ascending = false;
            }

            _model.Add(orderBy);

            if (FirstTokenType == TokenType.SequenceTerminator)
            {
                return;
            }
            else if (FirstTokenType == OrderingTokenType.Comma)
            {
                DiscardToken(OrderingTokenType.Comma);

                ParseOrdering();
            }
            else
                throw new DslParseException("syntax error");
        }
    }
}

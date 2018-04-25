using Axerrio.BB.DDD.Infrastructure.Query;
using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Query.API.Parser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Axerrio.BB.DDD.Infrastructure.Query.Specification;

namespace Axerrio.BB.DDD.Infrastructure.Query.ModelBinder
{
    //public class Orderings: List<(string KeySelector, bool Ascending)>
    //{

    //}

    public class OrderingParser : DslParser<OrderingParser, OrderingParser.Orderings, PrecedenceBasedRegexTokenizer>
    {
        public OrderingParser()
        {
        }

        public class Orderings : List<Ordering>
        {
        }

        public class Ordering
        {
            public string KeySelector { get; set; }
            public bool Ascending { get; set; }
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
            //(string keySelector, bool ascending) orderBy;
            Ordering orderBy = new Ordering();

            var propertyToken = ReadToken(OrderingTokenType.Property);
            DiscardToken(OrderingTokenType.Property);

            //orderBy.keySelector = propertyToken.Value;
            //orderBy.ascending = true; //default
            orderBy.KeySelector = propertyToken.Value;
            orderBy.Ascending = true; //default

            if (FirstTokenType == OrderingTokenType.Direction)
            {
                var directionToken = ReadToken(OrderingTokenType.Direction);
                DiscardToken(OrderingTokenType.Direction);

                if (directionToken.Value == "desc" || directionToken.Value == "descending")
                    orderBy.Ascending = false;
                    //orderBy.ascending = false;
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
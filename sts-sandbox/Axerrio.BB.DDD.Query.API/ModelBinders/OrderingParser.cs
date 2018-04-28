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
                new TokenDefinition(SpecificationTokenType.Comma, ",", 1),
                new TokenDefinition(SpecificationTokenType.Direction, "asc|ascending|desc|descending", 1),
                //new TokenDefinition(SpecificationTokenType.Property, @"[\p{L}\d\.]+", 2), 
                new TokenDefinition(SpecificationTokenType.Property, @"(?:[\w]+\.)*\w+", 2), //^(?:[\w]+\.)*\w+$
                //https://stackoverflow.com/questions/33711381/regular-expression-to-check-valid-property-name-in-c-sharp?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
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

            var propertyToken = ReadToken(SpecificationTokenType.Property);
            DiscardToken(SpecificationTokenType.Property);

            //orderBy.keySelector = propertyToken.Value;
            //orderBy.ascending = true; //default
            orderBy.KeySelector = propertyToken.Value;
            orderBy.Ascending = true; //default

            if (FirstTokenType == SpecificationTokenType.Direction)
            {
                var directionToken = ReadToken(SpecificationTokenType.Direction);
                DiscardToken(SpecificationTokenType.Direction);

                if (directionToken.Value == "desc" || directionToken.Value == "descending")
                    orderBy.Ascending = false;
                    //orderBy.ascending = false;
            }

            _model.Add(orderBy);

            if (FirstTokenType == TokenType.SequenceTerminator)
            {
                return;
            }
            else if (FirstTokenType == SpecificationTokenType.Comma)
            {
                DiscardToken(SpecificationTokenType.Comma);

                ParseOrdering();
            }
            else
                throw new DslParseException("syntax error");
        }
    }
}
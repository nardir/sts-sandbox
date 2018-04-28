using Axerrio.BB.DDD.Infrastructure.Query.ModelBinder;
using Axerrio.BB.DDD.Query.API.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.ModelBinders
{
    public class SelectorParser : DslParser<SelectorParser, List<string>, PrecedenceBasedRegexTokenizer>
    {
        public SelectorParser()
        {
        }

        protected override void BuildTokenDefinitions()
        {
            _tokenDefinitions = new List<TokenDefinition>()
            {
                new TokenDefinition(SpecificationTokenType.Comma, ",", 1),
                new TokenDefinition(SpecificationTokenType.Property, @"(?:[\w]+\.)*\w+", 1), //^(?:[\w]+\.)*\w+$
                //https://stackoverflow.com/questions/33711381/regular-expression-to-check-valid-property-name-in-c-sharp?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
            };

        }

        protected override void ParseDsl()
        {
            ParseSelector();
        }

        private void ParseSelector()
        {
            var propertyToken = ReadAndDiscardToken(SpecificationTokenType.Property);

            _model.Add(propertyToken.Value);

            if (FirstTokenType == TokenType.SequenceTerminator)
            {
                return;
            }
            else if (FirstTokenType == SpecificationTokenType.Comma)
            {
                //DiscardToken(SpecificationTokenType.Comma);
                DiscardToken();

                ParseSelector();
            }
            else
            {
                throw new DslParseException("syntax error");
            }
        }
    }
}
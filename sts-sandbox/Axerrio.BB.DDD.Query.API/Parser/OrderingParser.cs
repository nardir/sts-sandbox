using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class Ordering: List<(string KeySelector, bool Ascending)>
    {

    }

    public class OrderingParser : Parser<Ordering>
    {
        private Ordering _orderingModel;

        protected override void ParseDsl(Ordering dslModel)
        {
            //Init model
            _orderingModel = dslModel;

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

            _orderingModel.Add(orderBy);

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


    public abstract class Parser<TDslModel>
    {
        protected Stack<DslToken> _tokenSequence;
        protected DslToken _lookaheadFirst;
        protected DslToken _lookaheadSecond;

        public Parser()
        {
        }

        public void Parse(TDslModel dslModel, List<DslToken> tokens)
        {
            LoadTokenStack(tokens);
            PrepareLookaheads();

            ParseDsl(dslModel);

            DiscardToken(TokenType.SequenceTerminator);
        }

        protected abstract void ParseDsl(TDslModel dslModel);

        protected TokenType FirstTokenType => _lookaheadFirst?.TokenType;
        protected TokenType SecondTokenType => _lookaheadSecond?.TokenType;

        private void LoadTokenStack(List<DslToken> tokens)
        {
            _tokenSequence = new Stack<DslToken>();
            int count = tokens.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                _tokenSequence.Push(tokens[i]);
            }
        }

        private void PrepareLookaheads()
        {
            _lookaheadFirst = _tokenSequence.Pop();
            _lookaheadSecond = _tokenSequence.Pop();
        }

        protected DslToken ReadToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new DslParseException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            return _lookaheadFirst;
        }

        protected void DiscardToken()
        {
            _lookaheadFirst = _lookaheadSecond.Clone();

            if (_tokenSequence.Any())
                _lookaheadSecond = _tokenSequence.Pop();
            else
                _lookaheadSecond = new DslToken(TokenType.SequenceTerminator, string.Empty);
        }

        protected void DiscardToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new DslParseException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            DiscardToken();
        }
    }
}

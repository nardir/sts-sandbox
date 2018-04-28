using EnsureThat;
using System.Collections.Generic;
using System.Linq;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public abstract class DslParser<TParser, TModel, TTokenizer>
        where TModel : class
        where TTokenizer : ITokenizer, new()
        where TParser : DslParser<TParser, TModel, TTokenizer>, new()
    {
        protected ITokenizer _tokenizer;
        protected List<TokenDefinition> _tokenDefinitions;

        protected Stack<DslToken> _tokenSequence;
        protected DslToken _lookaheadFirst;
        protected DslToken _lookaheadSecond;
        protected TModel _model;

        public DslParser()
        {
            BuildTokenDefinitions();

            _tokenizer = new TTokenizer();
        }

        //public void Parse(TDslModel dslModel, List<DslToken> tokens)
        //protected void Parse(IEnumerable<DslToken> tokens)
        //{
        //    LoadTokenStack(tokens.ToList());
        //    PrepareLookaheads();

        //    ParseDsl();

        //    DiscardToken(TokenType.SequenceTerminator);
        //}

        protected void ParseText(TModel model, string text)
        {
            _model = EnsureArg.IsNotNull(model, nameof(model));

            var tokens = _tokenizer.Tokenize(text, _tokenDefinitions);

            LoadTokenStack(tokens.ToList());
            PrepareLookaheads();

            ParseDsl();

            DiscardToken(TokenType.SequenceTerminator);
        }

        public static void Parse(TModel model, string text)
        {
            var parser = new TParser();

            parser.ParseText(model, text);
        }

        protected abstract void ParseDsl();
        protected abstract void BuildTokenDefinitions();

        protected TokenType FirstTokenType => _lookaheadFirst?.TokenType;
        protected TokenType SecondTokenType => _lookaheadSecond?.TokenType;

        //private void LoadTokenStack(List<DslToken> tokens)
        private void LoadTokenStack(List<DslToken> tokens)
        {
            _tokenSequence = new Stack<DslToken>();

            //tokens.Reverse().ToList().ForEach(t => _tokenSequence.Push(t));

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

            //return _lookaheadFirst;
            return ReadToken();
        }

        protected DslToken ReadToken()
        {
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

        protected DslToken ReadAndDiscardToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
                throw new DslParseException(string.Format("Expected {0} but found: {1}", tokenType.ToString().ToUpper(), _lookaheadFirst.Value));

            return ReadAndDiscardToken();
        }

        protected DslToken ReadAndDiscardToken()
        {
            var token = ReadToken();

            DiscardToken();

            return token;
        }
    }
}

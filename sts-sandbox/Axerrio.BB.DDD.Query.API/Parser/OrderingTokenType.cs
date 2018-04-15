using Axerrio.BB.DDD.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Parser
{
    public class OrderingTokenType: TokenType //Enumeration<OrderingTokenType>, ITokenType
    {
        public static OrderingTokenType Comma = new OrderingTokenType(1, nameof(Comma));
        public static OrderingTokenType Direction = new OrderingTokenType(2, nameof(Direction));
        public static OrderingTokenType Property = new OrderingTokenType(3, nameof(Property));

        static OrderingTokenType()
        {
            _items.AddRange(new[] { Comma, Direction, Property });
        }

        protected OrderingTokenType()
        {
        }

        protected OrderingTokenType(int id, string name) : base(id, name)
        {
        }
    }
}

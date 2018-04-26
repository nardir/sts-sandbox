using Axerrio.BB.DDD.Domain.Abstractions;
using Axerrio.BB.DDD.Query.API.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.Query.ModelBinder
{
    public class SpecificationTokenType: TokenType //Enumeration<OrderingTokenType>, ITokenType
    {
        public static SpecificationTokenType Comma = new SpecificationTokenType(1, nameof(Comma));
        public static SpecificationTokenType Direction = new SpecificationTokenType(2, nameof(Direction));
        public static SpecificationTokenType Property = new SpecificationTokenType(3, nameof(Property));

        static SpecificationTokenType()
        {
            _items.AddRange(new[] { Comma, Direction, Property });
        }

        protected SpecificationTokenType()
        {
        }

        protected SpecificationTokenType(int id, string name) : base(id, name)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Helpers
{
    public class MemberNameExtractor : ExpressionVisitor
    {
        private string _memberName = null;

        protected string MemberName => _memberName;

        public static string Extract(LambdaExpression expression)
        {
            if (expression == null)
                return null;

            var extractor = new MemberNameExtractor();

            extractor.Visit(expression);

            return extractor.MemberName;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _memberName = node.Member.Name;

            return node;
        }
    }
}

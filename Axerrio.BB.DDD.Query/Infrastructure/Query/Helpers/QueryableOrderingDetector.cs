using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Helpers
{
    public class QueryableOrderingDetector: ExpressionVisitor
    {
        protected bool HasOrderBy { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) &&
                (node.Method.Name == nameof(Queryable.OrderBy) || node.Method.Name == nameof(Queryable.OrderByDescending)))
            {
                HasOrderBy = true;

                return node;
            }

            return base.VisitMethodCall(node);
        }

        public static bool HasOrdering(Expression expression)
        {
            EnsureArg.IsNotNull(expression, nameof(expression));

            var detector = new QueryableOrderingDetector();

            detector.Visit(expression);

            return detector.HasOrderBy;
        }
    }
}

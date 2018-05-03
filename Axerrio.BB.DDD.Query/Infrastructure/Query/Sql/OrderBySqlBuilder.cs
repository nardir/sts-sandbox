using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static Axerrio.BB.DDD.Infrastructure.Query.Specification;

namespace Axerrio.BB.DDD.Infrastructure.Query.Sql
{

    public class OrderBySqlBuilder: SqlBuilder
    {
        #region ctor

        public OrderBySqlBuilder(ISpecification specification) : base(specification)
        {
        }

        public OrderBySqlBuilder(ISpecification specification, Func<MemberInfo, string> resolveColumn) : base(specification, resolveColumn)
        {
        }

        #endregion

        public override string Build()
        {
            if (!Specification.HasOrdering)
                return null;

            var sql = new StringBuilder();
            bool first = true;

            foreach (var ordering in Specification.Orderings.Cast<Ordering>())
            {
                if (!first)
                {
                    sql.Append(", ");
                }

                var keySelectorMember = MembersExtractor.Extract(ordering.KeySelectorLambda).SingleOrDefault();
                var column = ResolveColumn(keySelectorMember);
                //var column = ResolveColumn(ordering.KeySelectorMember);

                if (string.IsNullOrWhiteSpace(column))
                    throw new ArgumentNullException();

                sql.Append($"{column}");

                if (!ordering.Ascending)
                    sql.Append(" DESC");

                first = false;
            }

            return sql.ToString();
        }
    }
}
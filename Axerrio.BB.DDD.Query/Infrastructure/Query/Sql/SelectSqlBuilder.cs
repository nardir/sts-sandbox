using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Helpers;

namespace Axerrio.BB.DDD.Infrastructure.Query.Sql
{
    public class SelectSqlBuilder : SqlBuilder
    {
        #region ctor

        public SelectSqlBuilder(ISpecification specification) : base(specification)
        {
        }

        public SelectSqlBuilder(ISpecification specification, Func<MemberInfo, string> resolveColumn) : base(specification, resolveColumn)
        {
        }

        #endregion

        public override string Build()
        {
            if (!Specification.HasSelector)
                return null;

            var sql = new StringBuilder();
            bool first = true;

            var selectorMembers = MembersExtractor.Extract(Specification.Selector);

            foreach (var selector in selectorMembers)
            {
                if (!first)
                {
                    sql.Append(", ");
                }

                var column = ResolveColumn(selector);

                if (string.IsNullOrWhiteSpace(column))
                    throw new ArgumentNullException();

                sql.Append($"{column}");

                first = false;
            }

            return sql.ToString();
        }
    }
}

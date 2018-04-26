using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static Axerrio.BB.DDD.Infrastructure.Query.Specification;

namespace Axerrio.BB.DDD.Infrastructure.Query.Sql
{
    public class OrderBySqlBuilder
    {
        private ISpecification _specification;

        private Func<MemberInfo, string> _resolveColumn = m => $"[{m.Name}]" ;

        public Func<MemberInfo, string> ResolveColumn
        {
            get => _resolveColumn;

            private set => _resolveColumn = EnsureArg.IsNotNull(value, nameof(ResolveColumn));
        }

        #region ctor

        public OrderBySqlBuilder(ISpecification specification): this(specification, null)
        {
        }

        public OrderBySqlBuilder(ISpecification specification, Func<MemberInfo, string> resolveColumn)
        {
            _specification = EnsureArg.IsNotNull(specification, nameof(specification));

            if (resolveColumn != null)
                ResolveColumn = resolveColumn;
        }

        #endregion

        public OrderBySqlBuilder WithResolveColumn(Func<MemberInfo, string> resolveColumn)
        {
            ResolveColumn = resolveColumn;

            return this;
        }

        public string Build()
        {
            if (!_specification.HasOrdering)
                return null;

            var sql = new StringBuilder();
            bool first = true;

            foreach(var ordering in _specification.Orderings.Cast<Ordering>())
            {
                if (!first)
                {
                    sql.Append(", ");
                }

                var column = ResolveColumn(ordering.KeySelectorMember);

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

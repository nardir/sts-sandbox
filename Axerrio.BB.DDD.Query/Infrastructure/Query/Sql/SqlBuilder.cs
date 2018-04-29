using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using EnsureThat;
using System;
using System.Reflection;

namespace Axerrio.BB.DDD.Infrastructure.Query.Sql
{
    public abstract class SqlBuilder
    {
        private Func<MemberInfo, string> _resolveColumn = m => $"[{m.Name}]";
        private ISpecification _specification;

        #region ctor

        public SqlBuilder(ISpecification specification) : this(specification, null)
        {
        }

        public SqlBuilder(ISpecification specification, Func<MemberInfo, string> resolveColumn)
        {
            _specification = EnsureArg.IsNotNull(specification, nameof(specification));

            if (resolveColumn != null)
                ResolveColumn = resolveColumn;
        }

        #endregion

        public Func<MemberInfo, string> ResolveColumn
        {
            get => _resolveColumn;

            protected set => _resolveColumn = EnsureArg.IsNotNull(value, nameof(ResolveColumn));
        }

        public ISpecification Specification => _specification;

        public SqlBuilder WithResolveColumn(Func<MemberInfo, string> resolveColumn)
        {
            ResolveColumn = resolveColumn;

            return this;
        }

        public abstract string Build();
    }
}
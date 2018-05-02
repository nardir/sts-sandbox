using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Helpers;
using Axerrio.BB.DDD.Infrastructure.Query.Validation;
using EnsureThat;
using FluentValidation;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query
{
    public abstract class Specification: ISpecification
    {
        protected static Dictionary<string, object> _lambdaValues = new Dictionary<string, object>();

        static Specification()
        {
            Expression<Func<string, string, bool>> likeLambda = (m, p) => EF.Functions.Like(m, p);
            //Expression<Func<DateTime, DateTime, int>> dateDiffDayLambda = (sd, ed) => EF.Functions.DateDiffDay(sd, ed); //EF Core 2.1

            _lambdaValues.Add("@like", likeLambda);
            //_lambdaArgs.Add("@datediffday", dateDiffDayLambda);
        }

        protected LambdaExpression ParseLambda(Type resultType, string expression, params object[] values)
        {
            int i = 0;
            var extendedValues = new Dictionary<string, object>(_lambdaValues);

            Array.ForEach(values, v => extendedValues.Add($"@p{i++}", v));

            return DynamicExpressionParser.ParseLambda(EntityType, resultType, expression, extendedValues);
        }

        public Type EntityType { get; private set; }

        #region ctor

        public Specification(Type entityType)
        {
            EntityType = EnsureArg.IsNotNull(entityType, nameof(entityType));
        }

        #endregion

        #region ordering 

        protected List<Ordering> _orderings = new List<Ordering>();

        public IReadOnlyList<IOrdering> Orderings => _orderings.AsReadOnly();

        public bool HasOrdering => _orderings.Any();

        protected ISpecification OrderBy(LambdaExpression keySelectorLambda, bool ascending)
        {
            //var member = MemberExtractor.Extract(keySelectorLambda);

            var ordering = new Ordering(keySelectorLambda, ascending);

            _orderings.Add(ordering);

            return this;
        }

        public class Ordering : IOrdering
        {
            private Lazy<MemberInfo> _member;

            public Ordering(LambdaExpression keySelectorLambda, bool ascending)
            {
                KeySelectorLambda = EnsureArg.IsNotNull(keySelectorLambda, nameof(keySelectorLambda));

                Ascending = ascending;

                _member = new Lazy<MemberInfo>(() => MembersExtractor.Extract(KeySelectorLambda).SingleOrDefault());
            }

            public LambdaExpression KeySelectorLambda { get; private set; }
            public bool Ascending { get; private set; }
            internal MemberInfo KeySelectorMember => _member.Value;
        }

        #endregion

        #region paging

        public bool HasPaging => (PageSize != null && PageIndex != null);

        public int? PageSize { get; protected set; } = null;

        public int? PageIndex { get; protected set; } = null;

        protected ISpecification Page(int pageSize, int pageIndex)
        {
            PageSize = EnsureArg.IsGt(pageSize, 0, nameof(pageSize));
            PageIndex = EnsureArg.IsGte(pageIndex, 0, nameof(pageIndex));

            return this;
        }

        #endregion

        #region selector

        public LambdaExpression Selector { get; protected set; }

        public bool HasSelector => Selector != null;

        #endregion
    }

    //public class OrderedSpecification<TEntity>: Specification<TEntity>
    //{
    //    protected OrderedSpecification(Specification<TEntity> specification)
    //    {
    //        EnsureArg.IsNotNull(specification, nameof(specification));

    //        Predicate = specification.Predicate;

    //        PageIndex = specification.PageIndex;
    //        PageSize = specification.PageSize;

    //        _orderings = specification.Orderings.Select(o => (Ordering)o).ToList();
    //    }

    //    public static OrderedSpecification<TEntity> Create(Specification<TEntity> specification)
    //    {
    //        if (specification == null)
    //            return null;

    //        return new OrderedSpecification<TEntity>(specification);
    //    }
    //}

    public class Specification<TEntity>: Specification, ISpecification<TEntity>, IOrderedSpecification<TEntity>
    {
        #region ctor

        public Specification(): base(typeof(TEntity))
        {
        }

        #endregion

        #region Predicate

        private ExpressionStarter<TEntity> _predicate = PredicateBuilder.New<TEntity>(true);

        private Expression<Func<TEntity, bool>> ParsePredicate(string predicate, params object[] args)
        {
            return (Expression<Func<TEntity, bool>>)ParseLambda(typeof(bool), predicate, args);
        }

        public Expression<Func<TEntity, bool>> Predicate
        {
            get => _predicate;

            protected set
            {
                _predicate = EnsureArg.IsNotNull(value, nameof(Predicate));
            }
        }

        public bool HasPredicate => _predicate.IsStarted;

        public ISpecification<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return And(predicate);
        }

        public ISpecification<TEntity> Where(string predicate, params object[] values)
        {
            return Where(ParsePredicate(predicate, values));
        }

        public ISpecification<TEntity> And(Expression<Func<TEntity, bool>> predicate)
        {
            EnsureArg.IsNotNull(predicate);

            _predicate = _predicate.And(predicate);

            return this;
        }

        public ISpecification<TEntity> And(string predicate, params object[] values)
        {
            return And(ParsePredicate(predicate, values));
        }

        public ISpecification<TEntity> Or(Expression<Func<TEntity, bool>> predicate)
        {
            EnsureArg.IsNotNull(predicate, nameof(predicate));

            _predicate = _predicate.Or(predicate);

            return this;
        }

        public ISpecification<TEntity> Or(string predicate, params object[] values)
        {
            return Or(ParsePredicate(predicate, values));
        }

        public static implicit operator Expression<Func<TEntity, bool>>(Specification<TEntity> right)
        {
            return right.Predicate;
        }

        public static implicit operator Func<TEntity, bool>(Specification<TEntity> right)
        {
            return right.Predicate.Compile();
        }

        public static implicit operator Specification<TEntity>(Expression<Func<TEntity, bool>> right)
        {
            return (Specification<TEntity>) new Specification<TEntity>().Where(right);
        }

        #endregion

        #region ordering

        public IOrderedSpecification<TEntity> OrderBy(string keySelector, bool ascending = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(keySelector, nameof(keySelector));
            
            OrderBy(ParseLambda(null, keySelector), ascending);

            return this;
        }

        public IOrderedSpecification<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool ascending = true)
        {
            EnsureArg.IsNotNull(keySelector, nameof(keySelector));

            OrderBy((LambdaExpression)keySelector, ascending);

            return this;
        }

        public IOrderedSpecification<TEntity> ThenBy(string keySelector, bool ascending = true)
        {
            return OrderBy(keySelector, ascending);
        }

        public IOrderedSpecification<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool ascending = true)
        {
            return OrderBy(keySelector, ascending);
        }

        #endregion

        #region paging

        public new ISpecification<TEntity> Page(int pageSize, int pageIndex)
        {
            base.Page(pageSize, pageIndex);

            return this;
        }

        #endregion

        #region selector

        public ISpecification<TEntity> Select(string keySelector)
        {
            EnsureArg.IsNotNullOrWhiteSpace(keySelector, nameof(keySelector));

            Selector = ParseLambda(null, $"new ({keySelector})");

            var members = MembersExtractor.Extract(Selector);

            return this;
        }

        public ISpecification<TEntity> Select<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            Selector = EnsureArg.IsNotNull(keySelector, nameof(keySelector));

            return this;
        }

        #endregion

        #region validation

        public void Validate(SpecificationValidationSettings validationSettings)
        {
            EnsureArg.IsNotNull(validationSettings);

            //use fluentvalidation
            var validator = new SpecificationValidator<TEntity>(validationSettings);

            var results = validator.Validate(this); //TODO Refactor comment

            validator.ValidateAndThrow(this);
        }

        public bool TryValidate(SpecificationValidationSettings validationSettings)
        {
            var valid = false;

            try
            {
                Validate(validationSettings);

                valid = true;
            }
            catch (Exception ex)
            {
                //return false;
            }

            return valid;
        }

        #endregion
    }
}

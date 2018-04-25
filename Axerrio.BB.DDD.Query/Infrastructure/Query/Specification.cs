using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Helpers;
using EnsureThat;
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
    public abstract class Specification
    {
        public class Ordering : IOrdering
        {
            public LambdaExpression KeySelectorLambda { get; set; }
            public bool Ascending { get; set; }
            public MemberInfo KeySelectorMember { get; set; }
        }
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
        private static Dictionary<string, object> _lambdaValues = new Dictionary<string, object>();

        static Specification()
        {
            Expression<Func<string, string, bool>> likeLambda = (m, p) => EF.Functions.Like(m, p);
            //Expression<Func<DateTime, DateTime, int>> dateDiffDayLambda = (sd, ed) => EF.Functions.DateDiffDay(sd, ed); //EF Core 2.1

            _lambdaValues.Add("@like", likeLambda);
            //_lambdaArgs.Add("@datediffday", dateDiffDayLambda);
        }

        private static LambdaExpression ParseLambda(Type resultType, string expression, params object[] values)
        {
            int i = 0;
            var extendedValues = new Dictionary<string, object>(_lambdaValues);

            Array.ForEach(values, v => extendedValues.Add($"@p{i++}", v));

            return DynamicExpressionParser.ParseLambda(typeof(TEntity), resultType, expression, extendedValues);
        }

        #region ctor
        public Specification()
        {
        }

        #endregion

        #region Predicate

        private ExpressionStarter<TEntity> _predicate = PredicateBuilder.New<TEntity>(true);

        private static Expression<Func<TEntity, bool>> ParsePredicate(string predicate, params object[] args)
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

        //public class Ordering
        //{
        //    public LambdaExpression KeySelectorLambda { get; set; }
        //    public bool Ascending { get; set; }
        //    public string KeySelector { get; set; }
        //}

        //private readonly List<(LambdaExpression KeySelectorLambda, bool Ascending, string KeySelector)> _orderings = new List<(LambdaExpression KeySelectorLambda, bool Ascending, string KeySelector)>();
        //private readonly List<Ordering> _orderings = new List<Ordering>();
        protected List<Ordering> _orderings = new List<Ordering>();

        public bool HasOrdering => _orderings.Any();

        //public IReadOnlyList<(LambdaExpression KeySelectorLambda, bool Ascending, string KeySelector)> Orderings => _orderings.AsReadOnly();
        public IReadOnlyList<IOrdering> Orderings => _orderings.AsReadOnly();

        public IOrderedSpecification<TEntity> OrderBy(string keySelector, bool ascending = true)
        {
            EnsureArg.IsNotNullOrWhiteSpace(keySelector, nameof(keySelector));
            
            return OrderBy(ParseLambda(null, keySelector), ascending);
        }

        public IOrderedSpecification<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool ascending = true)
        {
            EnsureArg.IsNotNull(keySelector, nameof(keySelector));

            return OrderBy((LambdaExpression)keySelector, ascending);
        }

        private IOrderedSpecification<TEntity> OrderBy(LambdaExpression keySelectorLambda, bool ascending)
        {
            var member = MemberExtractor.Extract(keySelectorLambda);

            _orderings.Add(new Ordering
                    {
                        Ascending = ascending,
                        KeySelectorLambda = keySelectorLambda,
                        KeySelectorMember = member
                    });

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

        public bool HasPaging => (PageSize != null && PageIndex != null);

        public int? PageSize { get; protected set; } = null;

        public int? PageIndex { get; protected set; } = null;

        public ISpecification<TEntity> Page(int pageSize, int pageIndex)
        {
            PageSize = EnsureArg.IsGt(pageSize, 0, nameof(pageSize));
            PageIndex = EnsureArg.IsGte(pageIndex, 0, nameof(pageIndex));

            return this;
        }

        #endregion
    }
}

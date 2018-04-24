using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
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
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query
{
    public class Specification<T>: ISpecification<T>
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

            return DynamicExpressionParser.ParseLambda(typeof(T), resultType, expression, extendedValues);
        }

        #region ctor
        public Specification()
        {
        }

        #endregion

        #region Predicate

        private ExpressionStarter<T> _predicate = PredicateBuilder.New<T>(true);

        private static Expression<Func<T, bool>> ParsePredicate(string predicate, params object[] args)
        {
            return (Expression<Func<T, bool>>)ParseLambda(typeof(bool), predicate, args);
        }

        public Expression<Func<T, bool>> Predicate => _predicate;

        public bool HasPredicate => _predicate.IsStarted;

        public ISpecification<T> Where(Expression<Func<T, bool>> predicate)
        {
            return And(predicate);
        }

        public ISpecification<T> Where(string predicate, params object[] values)
        {
            return Where(ParsePredicate(predicate, values));
        }

        public ISpecification<T> And(Expression<Func<T, bool>> predicate)
        {
            EnsureArg.IsNotNull(predicate);

            _predicate = _predicate.And(predicate);

            return this;
        }

        public ISpecification<T> And(string predicate, params object[] values)
        {
            return And(ParsePredicate(predicate, values));
        }

        public ISpecification<T> Or(Expression<Func<T, bool>> predicate)
        {
            EnsureArg.IsNotNull(predicate, nameof(predicate));

            _predicate = _predicate.Or(predicate);

            return this;
        }

        public ISpecification<T> Or(string predicate, params object[] values)
        {
            return Or(ParsePredicate(predicate, values));
        }

        public static implicit operator Expression<Func<T, bool>>(Specification<T> right)
        {
            return right.Predicate;
        }

        public static implicit operator Func<T, bool>(Specification<T> right)
        {
            return right.Predicate.Compile();
        }

        public static implicit operator Specification<T>(Expression<Func<T, bool>> right)
        {
            return (Specification<T>) new Specification<T>().Where(right);
        }

        #endregion

        #region ordering

        private readonly List<(LambdaExpression KeySelector, bool Ascending)> _orderings = new List<(LambdaExpression KeySelectorLambda, bool Ascending)>();

        public bool HasOrdering => _orderings.Any();

        public IReadOnlyList<(LambdaExpression KeySelector, bool Ascending)> Orderings => _orderings.AsReadOnly();



        public ISpecification<T> OrderBy(string keySelector, bool ascending = true)
        {
            //var keySelectorLambda = (Expression<Func<T, object>>) ParseLambda(typeof(object), keySelector);
            
            return OrderBy(ParseLambda(null, keySelector), ascending);
        }

        public ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool ascending = true)
        {
            return OrderBy((LambdaExpression)keySelector, ascending);
        }

        private ISpecification<T> OrderBy(LambdaExpression keySelector, bool ascending)
        {
            _orderings.Add((KeySelector: keySelector, Ascending: ascending));

            return this;
        }

        #endregion

        #region paging

        public bool HasPaging => (PageSize != null && PageIndex != null);

        public int? PageSize { get; private set; } = null;

        public int? PageIndex { get; private set; } = null;

        public ISpecification<T> Page(int pageSize, int pageIndex)
        {
            PageSize = EnsureArg.IsGt(pageSize, 0, nameof(pageSize));
            PageIndex = EnsureArg.IsGte(pageIndex, 0, nameof(pageIndex));

            return this;
        }

        #endregion
    }
}

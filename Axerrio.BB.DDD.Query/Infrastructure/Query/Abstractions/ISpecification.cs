using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Abstractions
{
    public interface ISpecification<T>
    {
        #region predicate

        Expression<Func<T, bool>> Predicate { get; }
        bool HasPredicate { get; }
        ISpecification<T> Where(Expression<Func<T, bool>> predicate);
        ISpecification<T> Where(string predicate, params object[] args);
        ISpecification<T> And(Expression<Func<T, bool>> predicate);
        ISpecification<T> And(string predicate, params object[] args);
        ISpecification<T> Or(Expression<Func<T, bool>> predicate);
        ISpecification<T> Or(string predicate, params object[] args);

        #endregion

        #region ordering

        bool HasOrdering { get; }
        IReadOnlyList<(LambdaExpression KeySelector, bool Ascending)> Orderings { get; }
        ISpecification<T> OrderBy(string keySelector, bool ascending = true);
        ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool ascending = true);

        #endregion
    }
}

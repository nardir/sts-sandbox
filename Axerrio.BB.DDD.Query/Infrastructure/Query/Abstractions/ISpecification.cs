using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Abstractions
{
    public interface IOrderedSpecification<TEntity>: ISpecification<TEntity>
    {
        IOrderedSpecification<TEntity> ThenBy(string keySelector, bool ascending = true);
        IOrderedSpecification<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool ascending = true);
    }

    public interface ISpecification<TEntity>
    {
        #region predicate

        Expression<Func<TEntity, bool>> Predicate { get; }
        bool HasPredicate { get; }
        ISpecification<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        ISpecification<TEntity> Where(string predicate, params object[] args);
        ISpecification<TEntity> And(Expression<Func<TEntity, bool>> predicate);
        ISpecification<TEntity> And(string predicate, params object[] args);
        ISpecification<TEntity> Or(Expression<Func<TEntity, bool>> predicate);
        ISpecification<TEntity> Or(string predicate, params object[] args);

        #endregion

        #region ordering

        bool HasOrdering { get; }
        //IReadOnlyList<(LambdaExpression KeySelectorLambda, bool Ascending, string KeySelector)> Orderings { get; }
        IReadOnlyList<IOrdering> Orderings { get; }
        IOrderedSpecification<TEntity> OrderBy(string keySelector, bool ascending = true);
        IOrderedSpecification<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool ascending = true);

        #endregion

        #region paging

        bool HasPaging { get; }
        int? PageSize { get; }
        int? PageIndex { get; }
        ISpecification<TEntity> Page(int pageSize, int pageIndex);

        #endregion
    }
}

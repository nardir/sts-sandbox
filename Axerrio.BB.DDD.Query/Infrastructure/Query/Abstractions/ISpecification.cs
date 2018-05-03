using Axerrio.BB.DDD.Infrastructure.Query.Validation;
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

    public interface ISpecification
    {
        Type EntityType { get; }

        bool HasOrdering { get; }
        IReadOnlyList<IOrdering> Orderings { get; }

        bool HasPaging { get; }
        int? PageSize { get; }
        int? PageIndex { get; }

        LambdaExpression Selector { get; }

        bool HasSelector { get; }

    }

    public interface ISpecification<TEntity>: ISpecification
    {
        #region predicate

        Expression<Func<TEntity, bool>> Filter { get; }
        bool HasFilter { get; }
        ISpecification<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        ISpecification<TEntity> Where(string predicate, params object[] args);
        ISpecification<TEntity> And(Expression<Func<TEntity, bool>> predicate);
        ISpecification<TEntity> And(string predicate, params object[] args);
        ISpecification<TEntity> Or(Expression<Func<TEntity, bool>> predicate);
        ISpecification<TEntity> Or(string predicate, params object[] args);

        #endregion

        #region ordering

        IOrderedSpecification<TEntity> OrderBy(string keySelector, bool ascending = true);
        IOrderedSpecification<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool ascending = true);

        #endregion

        #region paging

        ISpecification<TEntity> Page(int pageSize, int pageIndex);

        #endregion

        #region selector

        ISpecification<TEntity> Select(string keySelector);
        ISpecification<TEntity> Select<TKey>(Expression<Func<TEntity, TKey>> keySelector);

        #endregion

        #region validation

        void Validate(SpecificationValidationSettings validationSettings);
        bool TryValidate(SpecificationValidationSettings validationSettings);

        #endregion
    }
}

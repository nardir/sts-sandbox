using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Query.Helpers;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Extensions
{
    public static class QueryableSpecificationExtensions
    {
        public static IQueryable<TResult> ApplySpecificationSelector<TSource, TResult>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            return source.ApplySpecificationSelector(specification).Cast<TResult>();
        }

        public static IQueryable ApplySpecificationSelector<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasSelector)
                return source;

            var selectorMethod = ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(typeof(TSource), specification.Selector.Body.Type);
            
            var projection = selectorMethod.Invoke(null, new object[] { source, specification.Selector }) as IQueryable;

            return projection;
        }

        public static IQueryable<TSource> ApplySpecificationFilter<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasFilter)
                return source;

            return source.Where(specification.Filter);
        }

        //public static IQueryable<TSource> ApplySpecificationOrdering<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        public static IQueryable<TSource> ApplySpecificationOrdering<TSource>(this IQueryable<TSource> source, ISpecification specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasOrdering)
                return source;

            //typeof(IOrderedQueryable<Order>).IsAssignableFrom(query2.AsQueryable().Expression.Type);
            var hasOrdering = QueryableOrderingDetector.HasOrdering(source.Expression);

            if (hasOrdering)
                throw new InvalidOperationException("Cannot apply ordering is quaryable is already ordered");

            IOrderedQueryable<TSource> result = null;
            MethodInfo orderingMethod = null;

            foreach (var ordering in specification.Orderings)
            {
                if (hasOrdering == false)
                {
                    if (ordering.Ascending)
                    {
                        orderingMethod = ExpressionHelperMethods.QueryableOrderByGeneric.MakeGenericMethod(typeof(TSource), ordering.KeySelectorLambda.Body.Type);
                        
                    }
                    else
                    {
                        orderingMethod = ExpressionHelperMethods.QueryableOrderByDescendingGeneric.MakeGenericMethod(typeof(TSource), ordering.KeySelectorLambda.Body.Type);
                    }

                    hasOrdering = true;
                }
                else
                {
                    if (ordering.Ascending)
                    {
                        orderingMethod = ExpressionHelperMethods.QueryableThenByGeneric.MakeGenericMethod(typeof(TSource), ordering.KeySelectorLambda.Body.Type);
                    }
                    else
                    {
                        orderingMethod = ExpressionHelperMethods.QueryableThenByDescendingGeneric.MakeGenericMethod(typeof(TSource), ordering.KeySelectorLambda.Body.Type);
                    }
                }

                result = orderingMethod.Invoke(null, new object[] { result ?? source, ordering.KeySelectorLambda }) as IOrderedQueryable<TSource>;
            }

            return result;
        }

        //public static IQueryable<TSource> ApplySpecificationPaging<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        public static IQueryable<TSource> ApplySpecificationPaging<TSource>(this IQueryable<TSource> source, ISpecification specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasPaging) //Check ordering or default ordering of key in EF Core
                return source;

            return source.Skip(specification.PageIndex.Value * specification.PageSize.Value)
                .Take(specification.PageSize.Value);
        }

        //public static IQueryable<TSource> ApplySpecification<TSource>(this IOrderedQueryable<TSource> source, ISpecification<TSource> specification)
        //{
        //    return ApplySpecification((IQueryable<TSource>)source, specification);
        //}

        public static IQueryable<TSource> ApplySpecification<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            var query = source.ApplySpecificationFilter(specification);

            query = query.ApplySpecificationOrdering(specification);

            query = query.ApplySpecificationPaging(specification);

            return query;
        }

        public static IQueryable<TResult> ApplySpecification<TSource, TResult>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            var query = source.ApplySpecification(specification);

            return query.ApplySpecificationSelector<TSource, TResult>(specification);
        }
    }
}

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

        public static IQueryable<TSource> ApplySpecificationPredicate<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasPredicate)
                return source;

            return source.Where(specification.Predicate);
        }

        //public static IQueryable<TSource> ApplySpecificationOrdering<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        public static IQueryable<TSource> ApplySpecificationOrdering<TSource>(this IQueryable<TSource> source, ISpecification specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasOrdering)
                return source;

            IOrderedQueryable<TSource> result = null;
            MethodInfo orderByMethod = null;

            foreach (var order in specification.Orderings)
            {
                if (result == null && !(source is IOrderedQueryable<TSource>))
                {
                    if (order.Ascending)
                    {
                        orderByMethod = ExpressionHelperMethods.QueryableOrderByGeneric.MakeGenericMethod(typeof(TSource), order.KeySelectorLambda.Body.Type);
                        
                    }
                    else
                    {
                        orderByMethod = ExpressionHelperMethods.QueryableOrderByDescendingGeneric.MakeGenericMethod(typeof(TSource), order.KeySelectorLambda.Body.Type);
                    }
                }
                else
                {
                    if (order.Ascending)
                    {
                        orderByMethod = ExpressionHelperMethods.QueryableThenByGeneric.MakeGenericMethod(typeof(TSource), order.KeySelectorLambda.Body.Type);
                    }
                    else
                    {
                        orderByMethod = ExpressionHelperMethods.QueryableThenByDescendingGeneric.MakeGenericMethod(typeof(TSource), order.KeySelectorLambda.Body.Type);
                    }
                }

                result = orderByMethod.Invoke(null, new object[] { result ?? source, order.KeySelectorLambda }) as IOrderedQueryable<TSource>;
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

        public static IQueryable<TSource> ApplySpecification<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            var query = source.ApplySpecificationPredicate(specification);

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

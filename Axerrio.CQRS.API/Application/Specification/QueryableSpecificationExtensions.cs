using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
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

            var selector = specification.Selector;

            var projection = selector.Method.Invoke(null, new object[] { source, selector.Lambda }) as IQueryable;

            return projection;
        }

        public static IQueryable<TSource> ApplySpecificationPredicate<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasPredicate)
                return source;

            return source.Where(specification.Predicate);
        }

        public static IQueryable<TSource> ApplySpecificationOrder<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasOrder)
                return source;

            IQueryable<TSource> result = null;

            foreach (var order in specification.Order)
            {
                result = order.Method.Invoke(null, new object[] { result ?? source, order.Lambda }) as IQueryable<TSource>;
            }

            return result;
        }

        public static IQueryable<TSource> ApplySpecificationSkip<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasSkip)
                return source;

            return source.Skip(specification.Skip.Value);
        }

        public static IQueryable<TSource> ApplySpecificationTake<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            if (!specification.HasTake)
                return source;

            return source.Take(specification.Take.Value);
        }

        public static IQueryable<TSource> ApplySpecification<TSource>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            var query = source.ApplySpecificationPredicate(specification);

            query = query.ApplySpecificationOrder(specification);

            query = query.ApplySpecificationSkip(specification);

            query = query.ApplySpecificationTake(specification);

            return query;
        }

        public static IQueryable<TResult> ApplySpecification<TSource, TResult>(this IQueryable<TSource> source, ISpecification<TSource> specification)
        {
            var query = source.ApplySpecification(specification);

            return query.ApplySpecificationSelector<TSource, TResult>(specification);
        }
    }
}

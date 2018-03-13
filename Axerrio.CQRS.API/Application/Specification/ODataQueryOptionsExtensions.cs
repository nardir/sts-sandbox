using EnsureThat;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
{
    public static class ODataQueryOptionsExtensions
    {
        //public static ISpecification<T> ToSpecification<T>(this ODataQueryOptions options, ODataQuerySettings settings = null)
        //{
        //    //ODataValidationSettings validationSettings; // Not responsibility of this class
        //    settings = settings ?? new ODataQuerySettings();

        //    var query = new ODataQueryable<T>();

        //    options.ApplyTo(query, settings);

        //    return query.Specification;
        //}

        public static ISpecificationQueryable<T> ToSpecification<T>(this ODataQueryOptions options, ISpecificationQueryable<T> specificationQuery, ODataQuerySettings settings = null)
        {
            EnsureArg.IsNotNull(specificationQuery, nameof(specificationQuery));

            //ODataValidationSettings validationSettings; // Not responsibility of this class
            settings = settings ?? new ODataQuerySettings();

            if (options.OrderBy != null)
                settings.EnsureStableOrdering = false;

            //var query = new ODataQueryable<T>(specification);

            //options.ApplyTo(query, settings);
            options.ApplyTo(specificationQuery, settings);

            return specificationQuery;
        }

        public static ISpecification<T> ToSpecification<T>(this FilterQueryOption options, ISpecification<T> specification, ODataQuerySettings settings = null)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));

            settings = settings ?? new ODataQuerySettings();

            var query = new ODataQueryable<T>(specification);

            options.ApplyTo(query, settings);

            return query.Specification;
        }

    }
}

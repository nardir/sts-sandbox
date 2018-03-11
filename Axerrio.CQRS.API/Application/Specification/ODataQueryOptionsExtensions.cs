using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
{
    public static class ODataQueryOptionsExtensions
    {
        public static ISpecification<T> ToSpecification<T>(this ODataQueryOptions options, ODataQuerySettings settings = null)
        {
            //ODataValidationSettings validationSettings; // Not responsibility of this class
            settings = settings ?? new ODataQuerySettings();

            var query = new ODataQueryable<T>();

            options.ApplyTo(query, settings);

            //query.Specification.Skip = options.Skip?.Value;
            //query.Specification.Take = options.Top?.Value;

            return query.Specification;
        }

        public static ISpecification<T> ToSpecification<T>(this ODataQueryOptions options, ISpecification<T> specification, ODataQuerySettings settings = null)
        {
            //ODataValidationSettings validationSettings; // Not responsibility of this class

            throw new NotImplementedException();
        }

        public static ISpecification<T> ToSpecification<T>(this FilterQueryOption options, ODataQuerySettings settings = null)
        {
            settings = settings ?? new ODataQuerySettings();

            var query = new ODataQueryable<T>();

            options.ApplyTo(query, settings);

            return query.Specification;
        }

    }
}

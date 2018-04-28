using Axerrio.BB.DDD.Query.API.Parser;
using EnsureThat;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Exceptions;
using System.Threading.Tasks;
using static Axerrio.BB.DDD.Infrastructure.Query.ModelBinder.OrderingParser;

namespace Axerrio.BB.DDD.Infrastructure.Query.ModelBinder
{
    public class SpecificationModelBinder<T> : IModelBinder
    {
        private readonly ILogger<SpecificationModelBinder<T>> _logger;

        public SpecificationModelBinder(ILogger<SpecificationModelBinder<T>> logger)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            EnsureArg.IsNotNull(bindingContext, nameof(bindingContext));

            var modelType = bindingContext.ModelType;
            var request = bindingContext.HttpContext.Request;
            
            var options = request.Query.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value.FirstOrDefault());

            //var model = Activator.CreateInstance(modelType);
            var specification = new Specification<T>();

            //filter
            ApplyFilter(specification, bindingContext, options);

            //select

            //orderby
            ApplyOrdering(specification, bindingContext, options);

            //paging
            ApplyPaging(specification, bindingContext, options);


            if (bindingContext.ModelState.ErrorCount > 0)
            {
                ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(specification);

            return Task.CompletedTask;
        }

        private void ApplyFilter(Specification<T> specification, ModelBindingContext bindingContext, Dictionary<string, string> options)
        {
            options.TryGetValue("$filter", out string predicate);          

            if (!string.IsNullOrWhiteSpace(predicate))
            {
                try
                {
                    specification.Where(predicate);
                }
                //catch (ParseException exception)
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.ToString());

                    bindingContext.ModelState.TryAddModelError("filter", $"Invalid $filter expression {predicate} supplied");
                }
            }
        }

        private void ApplyOrdering(Specification<T> specification, ModelBindingContext bindingContext, Dictionary<string, string> options)
        {
            options.TryGetValue("$orderby", out string orderBy);

            if (string.IsNullOrWhiteSpace(orderBy))
                return;

            var orderings = new Orderings();

            try
            {
                OrderingParser.Parse(orderings, orderBy);
            }
            catch (DslParseException exception)
            {
                _logger.LogError(exception, exception.ToString());

                bindingContext.ModelState.TryAddModelError("orderby", $"Invalid $orderby expression {orderBy} supplied");

                return;
            }

            try
            {
                orderings.ForEach(o => specification.OrderBy(o.KeySelector, o.Ascending));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.ToString());

                bindingContext.ModelState.TryAddModelError("orderby", $"Invalid $orderby expression {orderBy} supplied");
            }
        }

        private void ApplySelect(Specification<T> specification, ModelBindingContext bindingContext, Dictionary<string, string> options)
        {
            options.TryGetValue("$select", out string keySelector);

            if (string.IsNullOrWhiteSpace(keySelector))
                return;
        }

        private void ApplyPaging(Specification<T> specification, ModelBindingContext bindingContext, Dictionary<string, string> options)
        {
            var containsPageSize = options.ContainsKey("$pagesize");
            var containsPageIndex = options.ContainsKey("$pageindex");

            if (!(containsPageSize && containsPageIndex))
                return; //no paging supplied, exit

            if (containsPageSize ^ containsPageIndex) //only size or index supplied, invalid paging
            {
                if (!containsPageSize)
                    bindingContext.ModelState.TryAddModelError("paging", $"Invalid paging, no page size supplied");

                if (!containsPageIndex)
                    bindingContext.ModelState.TryAddModelError("paging", $"Invalid paging, no page index supplied");
            }

            options.TryGetValue("$pagesize", out string rawPageSize);
            options.TryGetValue("$pageindex", out string rawPageIndex);

            if (int.TryParse(rawPageSize, out int pageSize) && int.TryParse(rawPageIndex, out int pageIndex))
            {
                try
                {
                    specification.Page(pageSize, pageIndex);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.ToString());

                    bindingContext.ModelState.TryAddModelError("Paging", $"Invalid paging (pagesize={rawPageSize}, pageindex={rawPageIndex}) supplied");
                }
            }
        }
    }
}

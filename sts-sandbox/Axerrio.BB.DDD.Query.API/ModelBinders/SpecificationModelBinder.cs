using EnsureThat;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Exceptions;
using System.Threading.Tasks;

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

            var options = request.Query.ToDictionary(p => p.Key, p => p.Value.FirstOrDefault());

            //var model = Activator.CreateInstance(modelType);
            var model = new Specification<T>();

            //filter
            ApplyFilter(model, bindingContext, options);

            //select
            //orderby
            //top
            //skip


            if (bindingContext.ModelState.ErrorCount > 0)
            {
                ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }

        private void ApplyFilter(Specification<T> specification, ModelBindingContext bindingContext, Dictionary<string, string> options)
        {
            string predicate = string.Empty;

            options.TryGetValue("$filter", out predicate);          

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

                    bindingContext.ModelState.TryAddModelError("$filter", "Invalid $filter expression supplied");
                }
            }
        }

    }
}

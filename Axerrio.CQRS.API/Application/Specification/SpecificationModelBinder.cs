using Axerrio.CQRS.API.Application.Query;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
{
    public class SpecificationModelBinder : IModelBinder
    {
        public SpecificationModelBinder()
        {
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            var request = bindingContext.HttpContext.Request;

            var spec = request.Query.ToDictionary(p => p.Key, p => p.Value.FirstOrDefault());

            //var model = Class.Activate(modelType, GetOptions(request), primitiveTypes.ToArray(), isCaseSensitive);
            //var model = new Specification<Customer>();
            var model = Activator.CreateInstance(modelType, spec);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }

    public class SpecificationModelBinder<T> : IModelBinder
    {
        public SpecificationModelBinder()
        {
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelType = bindingContext.ModelType;
            var request = bindingContext.HttpContext.Request;

            var spec = request.Query.ToDictionary(p => p.Key, p => p.Value.FirstOrDefault());

            //var model = Class.Activate(modelType, GetOptions(request), primitiveTypes.ToArray(), isCaseSensitive);
            //var model = new Specification<Customer>();
            var model = Activator.CreateInstance(modelType, spec);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}

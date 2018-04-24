using EnsureThat;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.ModelBinder
{
    public class SpecificationModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            EnsureArg.IsNotNull(context, nameof(context));

            var modelType = context.Metadata.ModelType; // Specification<Customer>
            var modelTypeInfo = modelType.GetTypeInfo();

            if (modelTypeInfo.IsGenericType && modelTypeInfo.GetGenericTypeDefinition() == typeof(Specification<>))
            {
                var genericTypeArgs = modelTypeInfo.GetGenericArguments(); // e.g. genericTypeArgs[0] = Customer

                var genericType = typeof(SpecificationModelBinder<>);
                var closedType = genericType.MakeGenericType(genericTypeArgs);

                return new BinderTypeModelBinder(closedType);
            }

            return null;
        }
    }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
{
    public class SpecificationModelBinderProvider : IModelBinderProvider
    {
        public SpecificationModelBinderProvider()
        {
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata.ModelType;
            var typeInfo = modelType.GetTypeInfo();

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Specification<>))
            {
                var genericTypeDefinition = typeInfo.GetGenericTypeDefinition();
                var genericTypeArgs = typeInfo.GetGenericArguments();

                var genericType = typeof(SpecificationModelBinder<>);
                var closedType = genericType.MakeGenericType(genericTypeArgs[0]);
                return (IModelBinder)Activator.CreateInstance(closedType);

                //return new SpecificationModelBinder();
            }

            return null;
        }
    }
}

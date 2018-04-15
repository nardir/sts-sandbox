using Axerrio.BB.DDD.Infrastructure.Query.ModelBinder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Extensions
{
    public static class MvcBuilderQueryExtensions
    {
        public static IMvcBuilder AddQueryOptions(this IMvcBuilder builder)
        {
            return builder.AddMvcOptions(o => o.ModelBinderProviders.Insert(0, new SpecificationModelBinderProvider()));
        }
    }
}

using Autofac;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.AutofacModules
{
    public class IntegrationEventHandlerModule<TAssembly> : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(TAssembly).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            builder.RegisterType<ForwardIntegrationEventHandler>();
        }
    }
}

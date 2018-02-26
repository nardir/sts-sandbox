using Autofac;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.API.Infrastructure.AutofacModules
{
    public class StoreAndForwardEventBusPublisherModule<TContext> : Autofac.Module
        where TContext : DbContext, IStoreAndForwardEventBusDbContext
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StoreAndForwardEventBusPublisher<TContext>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IntegrationEventsService<StoreAndForwardEventBusPublisher<TContext>>>()
                .As<IIntegrationEventsService>()
                .InstancePerLifetimeScope();
        }
    }
}


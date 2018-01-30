using Autofac;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Autofac
{
    public class EFCoreIntegrationEventsModule<TContext>: Module
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EFCoreStoreAndForwardEventBusFactory<TContext>>()
                .InstancePerLifetimeScope();

            builder.Register(context => 
                {
                    var factory = context.Resolve<EFCoreStoreAndForwardEventBusFactory<TContext>>();

                    return factory.Create();
                })
            .SingleInstance();
        }
    }
}
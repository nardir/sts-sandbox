﻿using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.Hosting;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Extensions
{
    public static class EFCoreIntegrationEventsServiceCollectionExtensions
    {
        public static IServiceCollection AddEFCoreStoreAndForwardIntegrationEventsServices<TContext, TForwardEventBus>(this IServiceCollection services, IConfiguration configuration = null)
            where TContext: DbContext, IIntegrationEventsDbContext
            where TForwardEventBus: IEventBusPublishOnly
        {
            services.AddEFCoreStoreAndForwardIntegrationEventsServices<TContext>(configuration);

            services.AddTransient<IIntegrationEventsForwarderService, IntegrationEventsForwarderService<TForwardEventBus>>();

            services.AddTransient<IJobFactory, JobFactory>();
            services.AddTransient<IntegrationEventsForwarderJob>();
            services.AddTransient<IntegrationEventsForwarderTriggerFactory>();
            //services.AddSingleton<IHostedService, TimedHostedService<IntegrationEventsForwarderJob, IntegrationEventsForwarderTriggerFactory>>();

            return services;
        }

        public static IServiceCollection AddEFCoreStoreAndForwardIntegrationEventsServices<TContext>(this IServiceCollection services, IConfiguration configuration = null)
            where TContext : DbContext, IIntegrationEventsDbContext
        {
            services.AddTransient<IEventBusPublishOnlyFactory, EventBusPublishOnlyFactory>();

            services.Configure<IntegrationEventsDatabaseOptions>(options =>
            {
                options = new IntegrationEventsDatabaseOptions();
            });

            services.Configure<IntegrationEventsQueueServiceOptions>(options =>
            {
                options = new IntegrationEventsQueueServiceOptions();
            });
            
            if (configuration != null)
            {
                //TODO : Config via key!!!!! Building Blocks
                var section = configuration.GetSection(nameof(IntegrationEventsQueueServiceOptions));
                services.Configure<IntegrationEventsQueueServiceOptions>(section);
            }

            services.AddTransient<IIntegrationEventsQueueService, EFCoreIntegrationEventsQueueService<TContext>>();

            services.AddScoped<IIntegrationEventsEnqueueService, EFCoreIntegrationEventsEnqueueService<TContext>>();
            services.AddScoped<StoreAndForwardEventBus>();
            services.AddScoped<IIntegrationEventsService, StoreAndForwardIntegrationEventsService<StoreAndForwardEventBus>>();

            return services;
        }

    }
}

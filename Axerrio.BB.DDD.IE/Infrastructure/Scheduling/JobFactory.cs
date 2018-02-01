using Autofac;
using EnsureThat;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Scheduling
{
    public class JobFactory : IJobFactory
    {
        //https://stackoverflow.com/questions/42157775/net-core-quartz-dependency-injection

        //private readonly IServiceProvider _serviceProvider;
        private readonly ILifetimeScope _lifetimeScope;

        public JobFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = EnsureArg.IsNotNull(lifetimeScope, nameof(lifetimeScope));
        }

        //public JobFactory(IServiceProvider serviceProvider)
        //{
        //    _serviceProvider = EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
        //}

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return _lifetimeScope.Resolve(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;

            disposable?.Dispose();
        }
    }
}

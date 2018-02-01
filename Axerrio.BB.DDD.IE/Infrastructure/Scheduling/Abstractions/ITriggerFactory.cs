using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Scheduling.Abstractions
{
    public interface ITriggerFactory
    {
        ITrigger Create();
    }
}

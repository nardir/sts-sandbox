﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions
{
    public interface IEventBusPublishOnlyFactory
    {
        IEventBusPublishOnly Create<TEventBus>() where TEventBus : IEventBusPublishOnly;
    }
}

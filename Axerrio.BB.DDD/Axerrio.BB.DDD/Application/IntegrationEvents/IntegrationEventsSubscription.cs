using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Application.IntegrationEvents
{
    public class IntegrationEventsSubscription
    {
        public bool IsDynamicHandler { get; private set; }
        public Type HandlerType { get; private set; }

        private IntegrationEventsSubscription(bool isDynamicHandler, Type handlerType)
        {
            IsDynamicHandler = isDynamicHandler;
            HandlerType = handlerType;
        }

        public static IntegrationEventsSubscription Create(bool isDynamicHandler, Type handlerType)
        {
            return new IntegrationEventsSubscription(isDynamicHandler, handlerType);
        }
    }
}

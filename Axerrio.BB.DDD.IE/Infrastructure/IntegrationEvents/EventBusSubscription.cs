using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class EventBusSubscription
    {
        public bool IsDynamicHandler { get; private set; }
        public Type HandlerType { get; private set; }

        private EventBusSubscription(bool isDynamicHandler, Type handlerType)
        {
            IsDynamicHandler = isDynamicHandler;
            HandlerType = handlerType;
        }

        public static EventBusSubscription Create(bool isDynamicHandler, Type handlerType)
        {
            return new EventBusSubscription(isDynamicHandler, handlerType);
        }
    }
}

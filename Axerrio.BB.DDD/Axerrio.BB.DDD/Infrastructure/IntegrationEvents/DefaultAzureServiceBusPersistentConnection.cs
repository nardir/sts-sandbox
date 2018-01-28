using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class DefaultAzureServiceBusPersistentConnection: IAzureServiceBusPersistentConnection
    {
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        private readonly ILogger<DefaultAzureServiceBusPersistentConnection> _logger;
        private readonly EventBusOptions _eventBusOptions;

        private ITopicClient _topicClient;

        //public DefaultAzureServiceBusPersistentConnection(ILogger<DefaultAzureServiceBusPersistentConnection> logger
        //    , ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder
        //    , IOptions<EventBusOptions> eventBusOptionsAccessor)
        //{
        //    _logger = EnsureArg.IsNotNull(logger, nameof(logger));
        //    _eventBusOptions = EnsureArg.IsNotNull(eventBusOptionsAccessor, nameof(eventBusOptionsAccessor)).Value;

        //    _serviceBusConnectionStringBuilder = EnsureArg.IsNotNull(serviceBusConnectionStringBuilder, nameof(serviceBusConnectionStringBuilder));
        //    _serviceBusConnectionStringBuilder.EntityPath = _eventBusOptions.BrokerName;
        //}

        public DefaultAzureServiceBusPersistentConnection(ILogger<DefaultAzureServiceBusPersistentConnection> logger
            , IOptions<EventBusOptions> eventBusOptionsAccessor)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _eventBusOptions = EnsureArg.IsNotNull(eventBusOptionsAccessor, nameof(eventBusOptionsAccessor)).Value;

            var cs = "Endpoint=sb://sts-sandbox.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=okAS5ZUVaWkRQaoF8u6e8m7ulHZ+cZgu7nB0suhVh+M=";

            _serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(_eventBusOptions.ConnectionString);
            //_serviceBusConnectionStringBuilder = new ServiceBusConnectionStringBuilder(cs);
            _serviceBusConnectionStringBuilder.EntityPath = _eventBusOptions.BrokerName;
        }

        public ISubscriptionClient CreateConsumerClient()
        {
            try
            {
                var client = new SubscriptionClient(_serviceBusConnectionStringBuilder, _eventBusOptions.SubscriptionName);

                return client;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ITopicClient CreatePublishClient()
        {
            if (_topicClient == null || _topicClient.IsClosedOrClosing)
            {
                try
                {
                    _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
                }
                catch (Exception ex)
                {

                }
            }

            return _topicClient;
        }
    }
}

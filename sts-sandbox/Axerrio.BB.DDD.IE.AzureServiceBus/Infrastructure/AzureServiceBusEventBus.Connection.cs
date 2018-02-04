using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.AzureServiceBus.Infrastructure
{
    public partial class AzureServiceBusEventBus
    {
        private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;

        private ITopicClient _topicClient;

        public void CreateTopicClient()
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
        }

        private ISubscriptionClient _subscriptionClient;

        public void CreateSubscriptionClient()
        {
            if (_subscriptionClient == null || _subscriptionClient.IsClosedOrClosing)
            {
                try
                {
                    _subscriptionClient = new SubscriptionClient(_serviceBusConnectionStringBuilder, _eventBusOptions.SubscriptionName);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }
}

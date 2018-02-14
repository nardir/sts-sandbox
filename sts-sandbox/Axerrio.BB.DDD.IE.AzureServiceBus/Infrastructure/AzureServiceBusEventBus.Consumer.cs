using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.AzureServiceBus.Infrastructure
{
    public partial class AzureServiceBusEventBus: IEventBusConsumer
    {
        #region IEventBusConsumer

        public Task StartConsumerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            StopConsumerAsync(cancellationToken).GetAwaiter().GetResult();

            CreateSubscriptionClient();

            _eventBusSubscriptionsService.EventRemoved += OnEventRemoved;
            _eventBusSubscriptionsService.EventAdded += OnEventAdded;

            RemoveDefaultRule();

            AddBrokerSubscriptions();

            var messageHandlerOptions = new MessageHandlerOptions(OnMessageReceivedException)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(OnMessageReceived, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public Task StopConsumerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _subscriptionClient?.CloseAsync();

            _eventBusSubscriptionsService.EventRemoved -= OnEventRemoved;
            _eventBusSubscriptionsService.EventAdded -= OnEventAdded;

            return Task.CompletedTask;
        }

        #endregion

        protected void OnEventAdded(object sender, string eventName)
        {
            AddBrokerSubscription(eventName);
        }

        protected void OnEventRemoved(object sender, string eventName)
        {
            try
            {
                _subscriptionClient
                 .RemoveRuleAsync(eventName)
                 .GetAwaiter()
                 .GetResult();

                if (_eventBusSubscriptionsService.IsEmpty)
                {
                    StopConsumerAsync().GetAwaiter().GetResult();
                }
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"The messaging entity { eventName } Could not be found.");
            }
        }

        private async Task OnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var eventName = message.Label; //$"{message.Label}{INTEGRATION_EVENT_SUFIX}";
            var eventMessage = Encoding.UTF8.GetString(message.Body);

            await _eventBusSubscriptionsService.DispatchEventAsync(eventName, eventMessage);

            // Complete the message so that it is not received again.
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task OnMessageReceivedException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return StartConsumerAsync();

            //Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            //var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            //Console.WriteLine("Exception context for troubleshooting:");
            //Console.WriteLine($"- Endpoint: {context.Endpoint}");
            //Console.WriteLine($"- Entity Path: {context.EntityPath}");
            //Console.WriteLine($"- Executing Action: {context.Action}");
            //return Task.CompletedTask;
        }

        private void AddBrokerSubscriptions()
        {
            foreach (var eventName in _eventBusSubscriptionsService.GetEvents())
            {
                AddBrokerSubscription(eventName);
            }
        }

        private void AddBrokerSubscription(string eventName)
        {
            try
            {
                _subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
            catch (ServiceBusException)
            {
                _logger.LogInformation($"An Azure Service Bus rule for {eventName} already exists.");
            }
        }

        private void RemoveDefaultRule()
        {
            try
            {
                _subscriptionClient
                 .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                 .GetAwaiter()
                 .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"The messaging entity { RuleDescription.DefaultRuleName } Could not be found.");
            }
        }
    }
}

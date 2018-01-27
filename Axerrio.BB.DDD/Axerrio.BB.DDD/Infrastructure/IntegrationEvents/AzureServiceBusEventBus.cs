using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    //https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions
    public class AzureServiceBusEventBus : IEventBus
    {
        private readonly ILogger<AzureServiceBusEventBus> _logger;
        private readonly IAzureServiceBusPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subscriptionManager;

        private ISubscriptionClient _subscriptionClient;

        public AzureServiceBusEventBus(ILogger<AzureServiceBusEventBus> logger
            , IAzureServiceBusPersistentConnection persistentConnection
            , IEventBusSubscriptionsManager subscriptionManager)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _persistentConnection = EnsureArg.IsNotNull(persistentConnection, nameof(persistentConnection));

            _subscriptionManager = EnsureArg.IsNotNull(subscriptionManager, nameof(subscriptionManager));

            _subscriptionManager.EventRemoved += OnEventRemoved;
            _subscriptionManager.EventAdded += OnEventAdded;

            CreateConsumer().GetAwaiter().GetResult();
        }

        private void OnEventAdded(object sender, string eventName)
        {
        }

        private void OnEventRemoved(object sender, string eventName)
        {
        }

        public async Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFIX, "");
            var eventName = @event.GetType().Name;

            var eventMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(eventMessage);

            var message = new Message
            {
                MessageId = @event.Id.ToString(),  //new Guid().ToString(),
                Body = body,
                Label = eventName,
            };

            var publishClient = _persistentConnection.CreatePublishClient();

            await publishClient.SendAsync(message);
        }

        private async Task CreateConsumer()
        {
            await _subscriptionClient?.CloseAsync();

            _subscriptionClient = _persistentConnection.CreateConsumerClient();

            RemoveDefaultRule();

            AddBrokerSubscriptions();

            var messageHandlerOptions = new MessageHandlerOptions(OnMessageReceivedException)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(OnMessageReceived, messageHandlerOptions);
        }

        private async Task OnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var eventName = message.Label; //$"{message.Label}{INTEGRATION_EVENT_SUFIX}";
            var eventMessage = Encoding.UTF8.GetString(message.Body);

            await _subscriptionManager.DispatchEventAsync(eventName, eventMessage);

            // Complete the message so that it is not received again.
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task OnMessageReceivedException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return CreateConsumer();

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
            foreach (var eventName in _subscriptionManager.Events)
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

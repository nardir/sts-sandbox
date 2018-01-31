using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.RabbitMQ.Infrastructure
{
    public partial class RabbitMQEventBus : IEventBusConsumer
    {
        private readonly IEventBusSubscriptionsService _eventBusSubscriptionsService;

        private IModel _consumerChannel;

        #region IEventBusConsumer

        public Task StartConsumeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            StopConsumeAsync(cancellationToken).GetAwaiter().GetResult();

            _consumerChannel = CreateModel();

            CreateExchange(_consumerChannel);

            _consumerChannel.QueueDeclare(queue: _eventBusOptions.SubscriptionName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _eventBusSubscriptionsService.EventRemoved += OnEventRemoved;
            _eventBusSubscriptionsService.EventAdded += OnEventAdded;

            AddBrokerSubscriptions();

            var consumer = new EventingBasicConsumer(_consumerChannel);

            consumer.Received += OnMessageReceived;

            _consumerChannel.CallbackException += OnMessageReceivedException;

            _consumerChannel.BasicConsume(queue: _eventBusOptions.SubscriptionName,
                                 autoAck: false,
                                 consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopConsumeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            _eventBusSubscriptionsService.EventRemoved -= OnEventRemoved;
            _eventBusSubscriptionsService.EventAdded -= OnEventAdded;

            _consumerChannel?.Dispose();

            return Task.CompletedTask;
        }

        #endregion

        private void OnMessageReceivedException(object sender, CallbackExceptionEventArgs e)
        {
            StartConsumeAsync().GetAwaiter().GetResult();
        }

        private void OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var eventMessage = Encoding.UTF8.GetString(e.Body);

            _eventBusSubscriptionsService.DispatchEventAsync(eventName, eventMessage).GetAwaiter().GetResult();

            _consumerChannel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }

        protected void OnEventAdded(object sender, string eventName)
        {
            AddBrokerSubscription(eventName);
        }

        protected void OnEventRemoved(object sender, string eventName)
        {
            using (var channel = CreateModel())
            {
                channel.QueueUnbind(queue: _eventBusOptions.SubscriptionName,
                    exchange: _eventBusOptions.BrokerName,
                    routingKey: eventName);

                if (_eventBusSubscriptionsService.IsEmpty)
                {
                    StopConsumeAsync().GetAwaiter().GetResult();
                }
            }
        }

        protected void AddBrokerSubscriptions()
        {
            foreach (var eventName in _eventBusSubscriptionsService.GetEvents())
            {
                AddBrokerSubscription(eventName);
            }
        }

        protected void AddBrokerSubscription(string eventName)
        {
            _consumerChannel.QueueBind(queue: _eventBusOptions.SubscriptionName,
                                 exchange: _eventBusOptions.BrokerName,
                                 routingKey: eventName);
        }
    }
}
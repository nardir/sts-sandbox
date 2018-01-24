using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Infrastructure.IntegrationEvents
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subscriptionManager;
        private readonly RabbitMQEventBusOptions _eventBusOptions;

        private IModel _consumerChannel;

        public RabbitMQEventBus(ILogger<RabbitMQEventBus> logger
            , IRabbitMQPersistentConnection persistentConnection
            , IEventBusSubscriptionsManager subscriptionManager
            , IOptions<RabbitMQEventBusOptions> eventBusOptionsAccessor)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _persistentConnection = EnsureArg.IsNotNull(persistentConnection, nameof(persistentConnection));
            _subscriptionManager = EnsureArg.IsNotNull(subscriptionManager, nameof(subscriptionManager));

            _eventBusOptions = EnsureArg.IsNotNull(eventBusOptionsAccessor, nameof(eventBusOptionsAccessor)).Value;

            _consumerChannel = CreateConsumerChannel();
        }

        #region IDisposable

        public void Dispose()
        {
            _consumerChannel?.Dispose();

            _subscriptionManager.Clear();
        }

        #endregion

        #region IEventBus

        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation($"Event {@event.Id} created on {@event.CreationTimestamp.ToShortTimeString()} is published");

            return Task.CompletedTask;
        }

        public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            throw new NotImplementedException();
        }

        #endregion

        private void CreateExchange(IModel channel)
        {
            channel.ExchangeDeclare(exchange: _eventBusOptions.Exchange, type: "direct", durable: true, autoDelete: false);
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            CreateExchange(channel);

            channel.QueueDeclare(queue: _eventBusOptions.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnEventReceived;

            channel.CallbackException += OnCallBackException;

            channel.BasicConsume(queue: _eventBusOptions.QueueName,
                                 autoAck: false,
                                 consumer: consumer);

            return channel;
        }

        private void OnCallBackException(object sender, CallbackExceptionEventArgs e)
        {
            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
        }

        private async void OnEventReceived(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var eventMessage = Encoding.UTF8.GetString(e.Body);

            await _subscriptionManager.DispatchEventAsync(eventName, eventMessage);

            _consumerChannel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }
    }
}

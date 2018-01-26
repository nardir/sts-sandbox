using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            _eventBusOptions = EnsureArg.IsNotNull(eventBusOptionsAccessor, nameof(eventBusOptionsAccessor)).Value;

            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _persistentConnection = EnsureArg.IsNotNull(persistentConnection, nameof(persistentConnection));

            _subscriptionManager = EnsureArg.IsNotNull(subscriptionManager, nameof(subscriptionManager));

            AddBrokerSubscriptions(_subscriptionManager.Events);

            _subscriptionManager.EventRemoved += OnEventRemoved;
            _subscriptionManager.EventAdded += OnEventAdded;

            CreateConsumerChannel();
        }

        private void OnEventAdded(object sender, string eventName)
        {
            AddBrokerSubscription(eventName);
        }

        private void OnEventRemoved(object sender, string eventName)
        {
            CheckConnection();

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _eventBusOptions.QueueName,
                    exchange: _eventBusOptions.Exchange,
                    routingKey: eventName);

                if (_subscriptionManager.IsEmpty)
                {
                    //_queueName = string.Empty; //TODO ?????????

                    _consumerChannel.Close();
                }
            }
        }

        private void CheckConnection()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
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
            CheckConnection();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_eventBusOptions.PublishRetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                CreateExchange(channel);

                var eventMessage = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(eventMessage);

                //TODO: Wel of geen message perrsistence
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                policy.Execute(() =>
                {
                    channel.BasicPublish(exchange: _eventBusOptions.Exchange,
                                     routingKey: eventName,
                                     basicProperties: null, //properties
                                     body: body);
                });
            }

            return Task.CompletedTask;
        }

        //public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
        //    where TIntegrationEvent : IntegrationEvent
        //    where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        //{
        //    var eventName = _subscriptionManager.GetEventName<TIntegrationEvent>();

        //    AddBrokerSubscription(eventName);

        //    _subscriptionManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        //}

        //public void Subscribe<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        //{
        //    AddBrokerSubscription(eventName);

        //    _subscriptionManager.AddSubscription<TIntegrationEventHandler>(eventName);
        //}

        //public void Unsubscribe<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler
        //{
        //    _subscriptionManager.RemoveSubscription<TIntegrationEventHandler>(eventName);
        //}

        //public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
        //    where TIntegrationEvent : IntegrationEvent
        //    where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        //{
        //    _subscriptionManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        //}

        #endregion

        private void CreateExchange(IModel channel)
        {
            channel.ExchangeDeclare(exchange: _eventBusOptions.Exchange, type: "direct", durable: true, autoDelete: false);
        }

        private void CreateConsumerChannel()
        {
            CheckConnection();

            _consumerChannel?.Dispose();

            _consumerChannel = _persistentConnection.CreateModel();

            CreateExchange(_consumerChannel);

            _consumerChannel.QueueDeclare(queue: _eventBusOptions.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var consumer = new EventingBasicConsumer(_consumerChannel);

            consumer.Received += OnEventReceived;

            _consumerChannel.CallbackException += OnCallBackException;

            _consumerChannel.BasicConsume(queue: _eventBusOptions.QueueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        private void OnCallBackException(object sender, CallbackExceptionEventArgs e)
        {
            CreateConsumerChannel();

            //_consumerChannel.Dispose();
            //_consumerChannel = CreateConsumerChannel();
        }

        private async void OnEventReceived(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var eventMessage = Encoding.UTF8.GetString(e.Body);

            await _subscriptionManager.DispatchEventAsync(eventName, eventMessage);
            //_subscriptionManager.DispatchEventAsync(eventName, eventMessage).GetAwaiter().GetResult();

            _consumerChannel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }

        private void AddBrokerSubscriptions(IEnumerable<string> eventNames)
        {
            CheckConnection();

            using (var channel = _persistentConnection.CreateModel())
            {
                foreach (var eventName in eventNames)
                {
                    CreateBinding(channel, eventName);
                }
            }
        }

        private void AddBrokerSubscription(string eventName)
        {
            CheckConnection();

            using (var channel = _persistentConnection.CreateModel())
            {
                CreateBinding(channel, eventName);
            }

            //var containsKey = _subscriptionManager.HasSubscriptionsForEvent(eventName);

            //if (!containsKey)
            //{
            //    CheckConnection();

            //    using (var channel = _persistentConnection.CreateModel())
            //    {
            //        CreateBinding(channel, eventName);
            //    }
            //}
        }

        private void CreateBinding(IModel channel, string eventName)
        {
            channel.QueueBind(queue: _eventBusOptions.QueueName,
                              exchange: _eventBusOptions.Exchange,
                              routingKey: eventName);
        }
    }
}

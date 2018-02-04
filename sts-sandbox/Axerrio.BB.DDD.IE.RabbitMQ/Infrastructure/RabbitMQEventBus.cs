using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.IE.RabbitMQ.Infrastructure
{
    public partial class RabbitMQEventBus: IEventBusPublisher
    {
        private readonly EventBusOptions _eventBusOptions;
        private readonly ILogger<RabbitMQEventBus> _logger;

        public RabbitMQEventBus(IEventBusSubscriptionsService eventBusSubscriptionsService, IOptions<EventBusOptions> eventBusOptionsAccessor, ILogger<RabbitMQEventBus> logger)
            : this(null, eventBusSubscriptionsService, eventBusOptionsAccessor, logger)
        {
        }

        public RabbitMQEventBus(IConnectionFactory connectionFactory, IEventBusSubscriptionsService eventBusSubscriptionsService, IOptions<EventBusOptions> eventBusOptionsAccessor, ILogger<RabbitMQEventBus> logger)
        {
            _eventBusOptions = EnsureArg.IsNotNull(eventBusOptionsAccessor, nameof(eventBusOptionsAccessor)).Value;
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _eventBusSubscriptionsService = EnsureArg.IsNotNull(eventBusSubscriptionsService, nameof(eventBusSubscriptionsService));

            _connectionFactory = connectionFactory ?? new ConnectionFactory { Uri = new Uri(_eventBusOptions.ConnectionString) };

        }

        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var eventName = IntegrationEvent.GetEventName(@event.GetType());
            var eventName = @event.GetEventName();
            var eventMessage = JsonConvert.SerializeObject(@event);

            return PublishAsync(eventName, @event.Id, eventMessage);
        }

        public Task PublishAsync(string eventName, Guid eventId, string eventMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_eventBusOptions.PublishRetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = CreateModel())
            {
                //var eventName = @event.GetType().Name;

                //var eventMessage = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(eventMessage);

                CreateExchange(channel);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = _eventBusOptions.PersistMessage;

                policy.Execute(() =>
                {
                    channel.BasicPublish(exchange: _eventBusOptions.BrokerName,
                                     routingKey: eventName,
                                     basicProperties: properties,
                                     body: body);
                });
            }

            return Task.CompletedTask;
        }
    }
}
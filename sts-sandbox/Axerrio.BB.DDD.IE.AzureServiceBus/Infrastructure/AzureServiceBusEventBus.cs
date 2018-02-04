using Axerrio.BB.DDD.Infrastructure.IntegrationEvents;
using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.AzureServiceBus.Infrastructure
{
    public partial class AzureServiceBusEventBus : IEventBusPublisher
    {
        private readonly EventBusOptions _eventBusOptions;
        private readonly ILogger<AzureServiceBusEventBus> _logger;

        public AzureServiceBusEventBus(IEventBusSubscriptionsService eventBusSubscriptionsService, IOptions<EventBusOptions> eventBusOptionsAccessor, ILogger<AzureServiceBusEventBus> logger)
            : this(null, eventBusSubscriptionsService, eventBusOptionsAccessor, logger)
        {
        }

        public AzureServiceBusEventBus(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder, IEventBusSubscriptionsService eventBusSubscriptionsService, IOptions<EventBusOptions> eventBusOptionsAccessor, ILogger<AzureServiceBusEventBus> logger)
        {
            _eventBusOptions = EnsureArg.IsNotNull(eventBusOptionsAccessor, nameof(eventBusOptionsAccessor)).Value;
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _eventBusSubscriptionsService = EnsureArg.IsNotNull(eventBusSubscriptionsService, nameof(eventBusSubscriptionsService));

            // "Endpoint=sb://sts-sandbox.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=okAS5ZUVaWkRQaoF8u6e8m7ulHZ+cZgu7nB0suhVh+M=";
            _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ?? new ServiceBusConnectionStringBuilder(_eventBusOptions.ConnectionString);

            _serviceBusConnectionStringBuilder.EntityPath = _eventBusOptions.BrokerName;
        }

        public Task PublishAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventName = @event.GetEventName();
            var eventMessage = JsonConvert.SerializeObject(@event);

            return PublishAsync(eventName, @event.Id, eventMessage);
        }

        public async Task PublishAsync(string eventName, Guid eventId, string eventMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFIX, "");
            //var eventName = @event.GetType().Name;

            //var eventMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(eventMessage);

            var message = new Message
            {
                MessageId = eventId.ToString(), //@event.Id.ToString(),  //new Guid().ToString(),
                Body = body,
                Label = eventName,
            };

            CreateTopicClient();

            await _topicClient.SendAsync(message);
        }
    }
}

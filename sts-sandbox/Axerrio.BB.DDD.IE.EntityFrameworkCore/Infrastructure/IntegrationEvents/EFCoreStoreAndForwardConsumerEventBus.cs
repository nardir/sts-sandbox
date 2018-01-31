using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreStoreAndForwardConsumerEventBus : IEventBusConsumer
    {
        private readonly EFCoreIntegrationEventsDatabaseOptions _integrationEventsDatabaseOptions;
        private readonly ILogger<EFCoreStoreAndForwardConsumerEventBus> _logger;
        private readonly IEventBusPublisher _eventBusPublisher;

        public EFCoreStoreAndForwardConsumerEventBus(IEventBusPublisher eventBusPublisher, ILogger<EFCoreStoreAndForwardConsumerEventBus> logger, IOptions<EFCoreIntegrationEventsDatabaseOptions> integrationEventsDatabaseOptionsAccessor)
        {
            _integrationEventsDatabaseOptions = EnsureArg.IsNotNull(integrationEventsDatabaseOptionsAccessor, nameof(EFCoreIntegrationEventsDatabaseOptions)).Value;
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _eventBusPublisher = EnsureArg.IsNotNull(eventBusPublisher, nameof(eventBusPublisher));
        }

        #region IEventBusConsumer

        public Task StartConsumeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task StopConsumeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        #endregion

        private string _requeueForBatchSql;

        private string RequeueForBatchSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_requeueForBatchSql))
                {
                    _requeueForBatchSql = $@"update {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                        , RequeuedTimestamp = @RequeueTimestamp
                                        , PublishBatchId = null
                                        , PublishAttempts = PublishAttempts - 1
                                        output inserted.*
                                        where PublishBatchId = @BatchId And [State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                        ";
                }

                return _requeueForBatchSql;
            }
        }


    }
}

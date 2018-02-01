using Axerrio.BB.DDD.Infrastructure.IntegrationEvents.Abstractions;
using Dapper;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public partial class StoreAndForwardEventBusForwarder : IEventBusForwarder
    {
        private readonly ILogger<StoreAndForwardEventBusForwarder> _logger;
        private readonly StoreAndForwardEventBusDatabaseOptions _databaseOptions;
        private readonly StoreAndForwardEventBusForwardOptions _forwardOptions;
        private readonly IEventBusPublisher _eventBusPublisher;
        private readonly Policy _retryPolicy;

        public StoreAndForwardEventBusForwarder(IEventBusPublisher eventBusPublisher
            , ILogger<StoreAndForwardEventBusForwarder> logger
            , IOptions<StoreAndForwardEventBusDatabaseOptions> databaseOptionsAccessor
            , IOptions<StoreAndForwardEventBusForwardOptions> forwardOptionsAccessor)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));
            _databaseOptions = EnsureArg.IsNotNull(databaseOptionsAccessor, nameof(databaseOptionsAccessor)).Value;
            _forwardOptions = EnsureArg.IsNotNull(forwardOptionsAccessor, nameof(forwardOptionsAccessor)).Value;
            _eventBusPublisher = EnsureArg.IsNotNull(eventBusPublisher, nameof(eventBusPublisher));

            _retryPolicy = CreatePolicy(_databaseOptions.RetryAttempts);
        }

        public async Task ForwardAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            bool cancel = false;
            var batchId = Guid.NewGuid();

            try
            {
                _logger.LogDebug($"Forwarding integration events for batch {batchId}");

                var requeuedItems = Enumerable.Empty<IntegrationEventsQueueItem>();
                var dequeuedItems = await DequeueEventsAsync(batchId, cancellationToken);

                foreach (var eventQueueItem in dequeuedItems)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancel = true;

                            break;
                        }

                        _logger.LogDebug($"Forwarding integration event for queue item {eventQueueItem.EventQueueItemId}");

                        await _eventBusPublisher.PublishAsync(eventQueueItem.IntegrationEvent);

                        await PublishEventAsync(eventQueueItem);

                        _logger.LogDebug($"Forwarded integration event for queue item {eventQueueItem.EventQueueItemId}");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Exception while forwarding integration event for queue item {eventQueueItem.EventQueueItemId}");

                        await RequeueOrFailEventAsync(eventQueueItem);
                    }
                }

                if (cancel)
                {
                    _logger.LogDebug($"Forwarding integration events cancelled for batch {batchId}");

                    //https://stackoverflow.com/questions/36426937/what-is-the-difference-between-wait-vs-getawaiter-getresult
                    //Mark all events with forward/dequeue batch id as NotPublished
                    //In case of cancel the reenqueue needs to finish
                    requeuedItems = RequeueEventsForBatchAsync(batchId).GetAwaiter().GetResult(); //In case of cancel the reenqueue needs to finish. GetAwaiter().GetResult() is used because it rearranges the stack trace in case of exception
                }

                _logger.LogDebug($"Forwarded integration events for batch {batchId} published #items {dequeuedItems.Count() - requeuedItems.Count()} requeued #items {requeuedItems.Count()}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Exception {exception.GetType().Name} with message ${exception.Message} detected while forwarding integration events for batch {batchId}");
            }
        }

        private Policy CreatePolicy(int retryAttempts = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retryAttempts,
                    sleepDurationProvider: retry => TimeSpan.FromMilliseconds(200 * retry),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        _logger.LogTrace(exception, $"Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retryAttempts}");
                    }
                );
        }

        private async Task<IEnumerable<IntegrationEventsQueueItem>> DequeueEventsAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Dequeueing integration events cancelled for batch {batchId}");

                return Enumerable.Empty<IntegrationEventsQueueItem>();
            }

            return (await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                _logger.LogDebug($"Dequeueing integration events for batch {batchId}");

                using (var connection = new SqlConnection(_databaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var queryParams = new { BatchId = batchId, DequeueTimestamp = DateTime.UtcNow };

                    var eventQueueItems = await connection.QueryAsync<IntegrationEventsQueueItem>(DequeueSql, queryParams);

                    _logger.LogDebug($"Dequeued integration events for batch {batchId} #queue items: {eventQueueItems.Count()}");

                    return eventQueueItems;
                }

            })).Result;
        }

        private Task RequeueOrFailEventAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Requeueing integration event cancelled for queue item: {eventQueueItem.EventQueueItemId}");

                return Task.CompletedTask;
            }

            var queryParams = new { eventQueueItem.EventQueueItemId, Timestamp = DateTime.UtcNow };
            string sql;
            IntegrationEventsQueueItemState state;

            sql = RequeueEventSql;
            state = IntegrationEventsQueueItemState.NotPublished;

            if (eventQueueItem.PublishAttempts >= _forwardOptions.MaxPublishAttempts)
            {
                sql = MarkEventAsPublishedFailedSql;
                state = IntegrationEventsQueueItemState.PublishedFailed;
            }

            return _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogDebug($"Requeueing integration event as {state} on attempt {eventQueueItem.PublishAttempts} for queue item {eventQueueItem.EventQueueItemId}");

                using (var connection = new SqlConnection(_databaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var rowsAffected = await connection.ExecuteAsync(sql, queryParams);

                    eventQueueItem.State = state;
                    eventQueueItem.PublishedTimestamp = queryParams.Timestamp;

                    _logger.LogDebug($"Requeued integration event as {state} on attempt {eventQueueItem.PublishAttempts} for queue item {eventQueueItem.EventQueueItemId}");
                }
            });
        }

        private Task PublishEventAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Publishing integration event cancelled for queue item {eventQueueItem.EventQueueItemId}");

                return Task.CompletedTask;
            }

            return _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogDebug($"Publishing integration event for queue item {eventQueueItem.EventQueueItemId} on attempt {eventQueueItem.PublishAttempts}");

                using (var connection = new SqlConnection(_databaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var param = new { eventQueueItem.EventQueueItemId, PublishedTimestamp = DateTime.UtcNow };

                    var rowsAffected = await connection.ExecuteAsync(MarkEventAsPublishedSql, param);

                    eventQueueItem.State = IntegrationEventsQueueItemState.Published;
                    eventQueueItem.PublishedTimestamp = param.PublishedTimestamp;

                    _logger.LogDebug($"Published integration event for queue item {eventQueueItem.EventQueueItemId} on attempt {eventQueueItem.PublishAttempts}");
                }
            });
        }

        private async Task<IEnumerable<IntegrationEventsQueueItem>> RequeueEventsForBatchAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Requeueing integration events cancelled for batch {batchId}");

                return Enumerable.Empty<IntegrationEventsQueueItem>();
            }

            return (await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                _logger.LogDebug($"Requeueing integration events for batch {batchId}");

                using (var connection = new SqlConnection(_databaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var queryParams = new { BatchId = batchId, RequeueTimestamp = DateTime.UtcNow };

                    var eventQueueItems = await connection.QueryAsync<IntegrationEventsQueueItem>(RequeueForBatchSql, queryParams);

                    _logger.LogDebug($"Requeueing integration events for batch {batchId} #queue items: {eventQueueItems.Count()}");

                    return eventQueueItems;
                }
            })).Result;
        }
    }
}
using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Dapper;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreIntegrationEventsDequeueService: IIntegrationEventsDequeueService
    {
        private readonly ILogger<EFCoreIntegrationEventsDequeueService> _logger;
        private readonly IntegrationEventsDatabaseOptions _integrationEventsDatabaseOptions;
        private readonly IntegrationEventsDequeueServiceOptions _integrationEventsDequeueServiceOptions;
        private readonly Policy _retryPolicy;

        public EFCoreIntegrationEventsDequeueService(ILogger<EFCoreIntegrationEventsDequeueService> logger
        , IOptions<IntegrationEventsDequeueServiceOptions> integrationEventsDequeueServiceOptionsAccessor
        , IOptions<IntegrationEventsDatabaseOptions> integrationEventsDatabaseOptionsAccessor)
        {
            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _integrationEventsDequeueServiceOptions = EnsureArg.IsNotNull(integrationEventsDequeueServiceOptionsAccessor, nameof(integrationEventsDequeueServiceOptionsAccessor)).Value;
            _integrationEventsDatabaseOptions = EnsureArg.IsNotNull(integrationEventsDatabaseOptionsAccessor, nameof(integrationEventsDatabaseOptionsAccessor)).Value;

            _retryPolicy = CreatePolicy();

            SetDequeueSql();
            SetRequeueEventSql();
            SetRequeueForBatchSql();
            SetMarkEventAsPublishedSql();
            SetMarkEventAsPublishedFailedSql();
        }

        public async Task<IEnumerable<IntegrationEventsQueueItem>> DequeueEventsAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Dequeueing integration events cancelled for batch {batchId}");

                return Enumerable.Empty<IntegrationEventsQueueItem>();
            }

            return (await _retryPolicy.ExecuteAndCaptureAsync(async () => 
            {
                _logger.LogDebug($"Dequeueing integration events for batch {batchId}");

                using (var connection = new SqlConnection(_integrationEventsDatabaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var queryParams = new { BatchId = batchId, DequeueTimestamp = DateTime.UtcNow };

                    var eventQueueItems = await connection.QueryAsync<IntegrationEventsQueueItem>(_dequeueSql, queryParams);

                    _logger.LogDebug($"Dequeued integration events for batch {batchId} #queue items: {eventQueueItems.Count()}");

                    return eventQueueItems;
                }

            })).Result;
        }

        public Task RequeueOrFailEventAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Requeueing integration event cancelled for queue item: {eventQueueItem.EventQueueItemId}");

                return Task.CompletedTask;
            }

            var queryParams = new { eventQueueItem.EventQueueItemId, Timestamp = DateTime.UtcNow };
            string sql;
            IntegrationEventsQueueItemState state;

            sql = _requeueEventSql;
            state = IntegrationEventsQueueItemState.NotPublished;

            if (eventQueueItem.PublishAttempts >= _integrationEventsDequeueServiceOptions.MaxPublishAttempts)
            {
                sql = _markEventAsPublishedFailedSql;
                state = IntegrationEventsQueueItemState.PublishedFailed;
            }

            return _retryPolicy.ExecuteAsync(async () => 
            {
                _logger.LogDebug($"Requeueing integration event as {state} on attempt {eventQueueItem.PublishAttempts} for queue item {eventQueueItem.EventQueueItemId}");

                using (var connection = new SqlConnection(_integrationEventsDatabaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var rowsAffected = await connection.ExecuteAsync(sql, queryParams);

                    eventQueueItem.State = state;
                    eventQueueItem.PublishedTimestamp = queryParams.Timestamp;

                    _logger.LogDebug($"Requeued integration event as {state} on attempt {eventQueueItem.PublishAttempts} for queue item {eventQueueItem.EventQueueItemId}");
                }
            });
        }

        public Task PublishEventAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Publishing integration event cancelled for queue item {eventQueueItem.EventQueueItemId}");

                return Task.CompletedTask;
            }

            return _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogDebug($"Publishing integration event for queue item {eventQueueItem.EventQueueItemId} on attempt {eventQueueItem.PublishAttempts}");

                    using (var connection = new SqlConnection(_integrationEventsDatabaseOptions.ConnectionString))
                    {
                        await connection.OpenAsync();

                        var param = new { eventQueueItem.EventQueueItemId, PublishedTimestamp = DateTime.UtcNow };

                        var rowsAffected = await connection.ExecuteAsync(_markEventAsPublishedSql, param);

                        eventQueueItem.State = IntegrationEventsQueueItemState.Published;
                        eventQueueItem.PublishedTimestamp = param.PublishedTimestamp;

                        _logger.LogDebug($"Published integration event for queue item {eventQueueItem.EventQueueItemId} on attempt {eventQueueItem.PublishAttempts}");
                    }
                });
        }

        public async Task<IEnumerable<IntegrationEventsQueueItem>> RequeueEventsForBatchAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Requeueing integration events cancelled for batch {batchId}");

                return Enumerable.Empty<IntegrationEventsQueueItem>();
            }

            return (await _retryPolicy.ExecuteAndCaptureAsync(async () =>
            {
                _logger.LogDebug($"Requeueing integration events for batch {batchId}");

                using (var connection = new SqlConnection(_integrationEventsDatabaseOptions.ConnectionString))
                {
                    await connection.OpenAsync();

                    var queryParams = new { BatchId = batchId, RequeueTimestamp = DateTime.UtcNow };

                    var eventQueueItems = await connection.QueryAsync<IntegrationEventsQueueItem>(_requeueForBatchSql, queryParams);

                    _logger.LogDebug($"Requeueing integration events for batch {batchId} #queue items: {eventQueueItems.Count()}");

                    return eventQueueItems;
                }
            })).Result;
        }

        private Policy CreatePolicy(int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromMilliseconds(200 * retry),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        _logger.LogTrace(exception, $"Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }

        #region Sql

        private string _requeueForBatchSql;

            private void SetRequeueForBatchSql()
            {
                _requeueForBatchSql = $@"update {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                        , RequeuedTimestamp = @RequeueTimestamp
                                        , PublishBatchId = null
                                        , PublishAttempts = PublishAttempts - 1
                                        output inserted.*
                                        where PublishBatchId = @BatchId And [State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                        ";
            }

            private string _requeueEventSql;

            private void SetRequeueEventSql()
            {
                _requeueEventSql = $@"update {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                        , RequeuedTimestamp = @Timestamp
                                        , PublishBatchId = null
                                        where EventQueueItemId = @EventQueueItemId
                                        ";
            }

            private string _dequeueSql;
            private void SetDequeueSql()
            {
                _dequeueSql = $@"with eqi as
                                    (
                                        select top {_integrationEventsDequeueServiceOptions.MaxEventsToDequeue} q.*
                                        from {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} as q with (rowlock, readpast)
                                        where q.[State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                    )
                                    update eqi set eqi.[State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                         , eqi.PublishAttempts = eqi.PublishAttempts + 1
                                         , eqi.PublishBatchId = @BatchId
                                         , eqi.LatestDequeuedTimestamp = @DequeueTimestamp
                                    output inserted.*";

            }

            private string _markEventAsPublishedFailedSql;

            private void SetMarkEventAsPublishedFailedSql()
            {
                _markEventAsPublishedFailedSql = $@"update {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.PublishedFailed}
                                                   , PublishedFailedTimestamp = @Timestamp
                                                   where EventQueueItemId = @EventQueueItemId
                                                   ";
            }

            private string _markEventAsPublishedSql;

            private void SetMarkEventAsPublishedSql()
            {
                _markEventAsPublishedSql = $@"update {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.Published}
                                        , PublishedTimestamp = @PublishedTimestamp
                                        where EventQueueItemId = @EventQueueItemId
                                        ";
            }

            #endregion

    }
}

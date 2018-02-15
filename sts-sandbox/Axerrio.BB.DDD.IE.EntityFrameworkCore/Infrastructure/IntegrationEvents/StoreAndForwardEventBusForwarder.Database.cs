using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public partial class StoreAndForwardEventBusForwarder
    {
        private string _requeueForBatchSql;
        private string _requeueEventSql;
        private string _dequeueSql;
        private string _markEventAsPublishedFailedSql;
        private string _markEventAsPublishedSql;
        private string _requeuePendingPublishingEventsSql;

        private string RequeueForBatchSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_requeueForBatchSql))
                {
                    _requeueForBatchSql = $@"update {_databaseOptions.Schema}.{_databaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                        , RequeuedTimestamp = @RequeueTimestamp
                                        , PublishBatchId = null
                                        , PublishAttempts = PublishAttempts - 1
                                        output inserted.*
                                        where PublishBatchId = @BatchId And [State] = {(int)IntegrationEventsQueueItemState.Publishing}";
                }

                return _requeueForBatchSql;
            }
        }

        private string RequeueEventSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_requeueEventSql))
                {
                    _requeueEventSql = $@"update {_databaseOptions.Schema}.{_databaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                        , RequeuedTimestamp = @Timestamp
                                        , PublishBatchId = null
                                        where EventQueueItemId = @EventQueueItemId";
                }

                return _requeueEventSql;
            }
        }

        private string DequeueSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_dequeueSql))
                {
                    _dequeueSql = $@"with eqi as
                                    (
                                        select top {_databaseOptions.MaxEventsToDequeue} q.*
                                        from {_databaseOptions.Schema}.{_databaseOptions.TableName} as q with (rowlock, readpast)
                                        where q.[State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                    )
                                    update eqi set eqi.[State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                         , eqi.PublishAttempts = eqi.PublishAttempts + 1
                                         , eqi.PublishBatchId = @BatchId
                                         , eqi.LatestDequeuedTimestamp = @DequeueTimestamp
                                    output inserted.*";
                }

                return _dequeueSql;
            }
        }

        private string MarkEventAsPublishedFailedSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_markEventAsPublishedFailedSql))
                {
                    _markEventAsPublishedFailedSql = $@"update {_databaseOptions.Schema}.{_databaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.PublishedFailed}
                                                   , PublishedFailedTimestamp = @Timestamp
                                                   where EventQueueItemId = @EventQueueItemId";
                }

                return _markEventAsPublishedFailedSql;
            }
        }



        private string MarkEventAsPublishedSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_markEventAsPublishedSql))
                {
                    _markEventAsPublishedSql = $@"update {_databaseOptions.Schema}.{_databaseOptions.TableName} set [State] = {(int)IntegrationEventsQueueItemState.Published}
                                        , PublishedTimestamp = @PublishedTimestamp
                                        where EventQueueItemId = @EventQueueItemId
                                        ";
                }

                return _markEventAsPublishedSql;
            }
        }

        private string RequeuePendingPublishingEventsSql
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_requeuePendingPublishingEventsSql))
                {
                    _requeuePendingPublishingEventsSql = $@"with eqi as
                                    (
                                        select q.*
                                        from {_databaseOptions.Schema}.{_databaseOptions.TableName} as q with (readuncommitted)
                                        where q.[State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                        and q.LatestDequeuedTimestamp //TODO JJ: verschil introduceren hier!
                                    )
                                    update eqi set eqi.[State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                         , eqi.PublishAttempts = eqi.PublishAttempts + 1
                                         , eqi.PublishBatchId = null
                                    output inserted.*";
                }

                return _requeuePendingPublishingEventsSql;
            }
        }
    }
}

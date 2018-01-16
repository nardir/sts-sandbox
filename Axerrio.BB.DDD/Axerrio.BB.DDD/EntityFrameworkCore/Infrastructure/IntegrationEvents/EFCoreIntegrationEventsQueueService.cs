using Axerrio.BB.DDD.Application.IntegrationEvents;
using Axerrio.BB.DDD.Application.IntegrationEvents.Abstractions;
using Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents.Abstractions;
using Dapper;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.EntityFrameworkCore.Infrastructure.IntegrationEvents
{
    public class EFCoreIntegrationEventsQueueService<TContext> : IIntegrationEventsQueueService
        where TContext : DbContext, IIntegrationEventsDbContext
    {
        private readonly TContext _context;
        private readonly ILogger<EFCoreIntegrationEventsQueueService<TContext>> _logger;
        private readonly IntegrationEventsDatabaseOptions _integrationEventsDatabaseOptions;
        private readonly IntegrationEventsQueueServiceOptions _integrationEventsQueueServiceOptions;

        public EFCoreIntegrationEventsQueueService(TContext context, ILogger<EFCoreIntegrationEventsQueueService<TContext>> logger, IOptionsSnapshot<IntegrationEventsQueueServiceOptions> integrationEventsQueueServiceOptionsAccessor)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _integrationEventsDatabaseOptions = EnsureArg.IsNotNull(_context.IntegrationEventsDatabaseOptions, nameof(_context.IntegrationEventsDatabaseOptions));

            _logger = EnsureArg.IsNotNull(logger, nameof(logger));

            _integrationEventsQueueServiceOptions = EnsureArg.IsNotNull(integrationEventsQueueServiceOptionsAccessor, nameof(integrationEventsQueueServiceOptionsAccessor)).Value;

            SetDequeueSql();
        }

        private string _dequeueSql;
        private void SetDequeueSql()
        {
                _dequeueSql = $@"with eqi as
                                (
                                    select top {_integrationEventsQueueServiceOptions.MaxEventsToDequeue} q.*
                                    from {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} as q with (rowlock, readpast)
                                    where q.[State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                )
                                update eqi set eqi.[State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                     , eqi.PublishAttempts = eqi.PublishAttempts + 1
                                     , eqi.PublishBatchId = @BatchId
                                     , eqi.LatestDequeuedTimestamp = @DequeueTimestamp
                                output inserted.*";

        }

        public async Task<IEnumerable<IntegrationEventsQueueItem>> DequeueEventsAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            //var items2 = _context.IntegrationEventsQueueItems.FromSql(_dequeueSql).ToListAsync(cancellationToken);

            return await executionStrategy.ExecuteAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested) //NR : check ipv meegeven aan diverse methods, hierdoor krijgen we controle over wat we doen ipv exceptions
                    return Enumerable.Empty<IntegrationEventsQueueItem>();

                var connection = _context.Database.GetDbConnection();

                //await connection.OpenAsync(cancellationToken);
                await connection.OpenAsync();

                //var cmd = new CommandDefinition(commandText: _dequeueSql, cancellationToken: cancellationToken);
                //var items = await connection.QueryAsync<IntegrationEventsQueueItem>(cmd);


                //TODO NR: Meegegeven dequeue batch id (GUID?), maakt het mogelijk om in 1 keer alles terug te zetten naar NotPublished
                var eventQueueitems = await connection.QueryAsync<IntegrationEventsQueueItem>(_dequeueSql, new { BatchId = batchId, DequeueTimestamp = DateTime.UtcNow });

                //var integrationEvents = items.Select(item => item.IntegrationEvent);

                connection.Close();

                return eventQueueitems;
            });

            //using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            //{
            //    await connection.OpenAsync(cancellationToken);

            //    try
            //    {
            //        var items = (await connection.QueryAsync<IntegrationEventsQueueItem>(sql)).Select(item => item.IntegrationEvent);

            //        return items;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}
        }

        public Task EnqueueEventAsync(IntegrationEventsQueueItem eventQueueItem)
        {
            EnsureArg.IsNotNull(eventQueueItem, nameof(eventQueueItem));

            return _context.IntegrationEventsQueueItems.AddAsync(eventQueueItem); //Async because we have a sequence
        }

        public Task MarkEventAsNotPublishedAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task MarkEventAsPublishedAsync(IntegrationEventsQueueItem eventQueueItem, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task RequeueEventsForBatchAsync(Guid batchId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}

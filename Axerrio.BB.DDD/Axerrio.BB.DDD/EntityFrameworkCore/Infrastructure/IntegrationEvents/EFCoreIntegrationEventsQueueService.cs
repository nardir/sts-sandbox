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
                _dequeueSql = $@"with qi as
                                (
                                    select top {_integrationEventsQueueServiceOptions.MaxEventsToDequeue} l.*
                                    from {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} as l with (rowlock, readpast)
                                    where l.[State] = {(int)IntegrationEventsQueueItemState.NotPublished}
                                )
                                update qi set qi.[State] = {(int)IntegrationEventsQueueItemState.Publishing}
                                output inserted.*";

        }

        public async Task<IEnumerable<IntegrationEvent>> DequeueEventsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //var sql = $@"with qi as
            //            (
            //                select top {_integrationEventsQueueServiceOptions.MaxEventsToDequeue} l.*
            //                from {_integrationEventsDatabaseOptions.Schema}.{_integrationEventsDatabaseOptions.TableName} as l with (rowlock, readpast)
            //                where l.[State] = {(int)IntegrationEventsQueueItemState.NotPublished}
            //            )
            //            update qi set qi.[State] = {(int)IntegrationEventsQueueItemState.Publishing}
            //            output inserted.*";

            var executionStrategy = _context.Database.CreateExecutionStrategy();

            //var items2 = _context.IntegrationEventsQueueItems.FromSql(_dequeueSql).ToListAsync(cancellationToken);

            return await executionStrategy.ExecuteAsync(async () =>
            {
                if (cancellationToken.IsCancellationRequested) //NR : check ipv meegeven aan diverse methods, hierdoor krijgen we controle over wat we doen ipv exceptions
                    return Enumerable.Empty<IntegrationEvent>();

                var connection = _context.Database.GetDbConnection();

                //await connection.OpenAsync(cancellationToken);
                await connection.OpenAsync();

                //var cmd = new CommandDefinition(commandText: _dequeueSql, cancellationToken: cancellationToken);
                //var items = await connection.QueryAsync<IntegrationEventsQueueItem>(cmd);


                //TODO NR: Meegegeven dequeue batch id (GUID?), maakt het mogelijk om in 1 keer alles terug te zetten naar NotPublished
                var items = await connection.QueryAsync<IntegrationEventsQueueItem>(_dequeueSql);

                var integrationEvents = items.Select(item => item.IntegrationEvent);

                connection.Close();

                return integrationEvents;

                //try
                //{
                //    var items = (await connection.QueryAsync<IntegrationEventsQueueItem>(sql)).Select(item => item.IntegrationEvent);

                //    return items;
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}
                //finally
                //{
                //    connection.Close();
                //}

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

        public void EnqueueEvent(IntegrationEvent @event)
        {
            EnsureArg.IsNotNull(@event, nameof(@event));

            _context.IntegrationEventsQueueItems.Add(new IntegrationEventsQueueItem(@event));
        }

        public Task MarkEventAsNotPublishedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task MarkEventAsPublishedAsync(IntegrationEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using Dapper;
using System.Data.Common;
using System.Data.SqlClient;
using Axerrio.DDD.BuildingBlocks;
using System.Threading.Tasks;
using Polly.Retry;
using Polly;

namespace Axerrio.DDD.QueueTest
{
    class Program
    {
        const string ConnectionString = "Server=(localdb)\\ProjectsV13;Initial Catalog=Ordering;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        static void Main(string[] args)
        {
            //EnqueueAsync().Wait();
            //SelectByEventIdAsync("5CE9DF89-C603-432A-8288-BCA07A0F3814").Wait();
            //SelectByEventIdAsync("5CE9DF89-C603-432A-8288-BCA07A0F38AA").Wait();

            //DequeueByEventIdAsync("5CE9DF89-C603-432A-8288-BCA07A0F3814").Wait();
            //DequeueByEventIdAsync("5CE9DF89-C603-432A-8288-BCA07A0F3814").Wait();
            //DequeueByEventIdAsync("5CE9DF89-C603-432A-8288-BCA07A0F38AA").Wait();

            //DequeueTop1Async().Wait();
            //DequeueTop1Async().Wait();

            //RetryPolicy retryPolicy = Policy
            //   .HandleAsync<SqlException>()
            //   .Retry(3);

            var retryPolicy = Policy.Handle<SqlException>().RetryAsync(3);

            IntegrationEventLogEntry entry = null;
            string eventId = "5CE9DF89-C603-432A-8288-BCA07A0F3814";

            //return await SelectByEventIdUsingPollyAsync(eventId)
            entry = retryPolicy.ExecuteAsync<IntegrationEventLogEntry>(async () => await SelectByEventIdUsingPollyAsync(eventId)).Result;
        }

        static async Task<IntegrationEventLogEntry> SelectByEventIdUsingPollyAsync(string eventId)
        {
            var sql = "select l.* from dbo.IntegrationEventLog as l where l.EventId = @EventId";

            IntegrationEventLogEntry entry = null;

            using (var connection = new SqlConnection(ConnectionString))
            {
                //connection.Open();
                await connection.OpenAsync();

                try
                {
                    //var rowsAffected = connection.Execute(sql, eventLogEntry);
                    entry = await connection.QueryFirstOrDefaultAsync<IntegrationEventLogEntry>(sql, new { EventId = Guid.Parse(eventId) });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return entry;
        }

        static async Task DequeueTop1Async()
        {
            var sql = @"with nqi as
                        (
                            select top 1 l.*
                            from dbo.IntegrationEventLog as l with (rowlock, readpast)
                            where l.[State] = 0
                        )
                        update nqi set nqi.[State] = 1
                        output inserted.*";

            using (var connection = new SqlConnection(ConnectionString))
            {
                //connection.Open();
                await connection.OpenAsync();

                try
                {
                    //var rowsAffected = connection.Execute(sql, eventLogEntry);
                    var entry = await connection.QueryFirstOrDefaultAsync<IntegrationEventLogEntry>(sql);
                }
                catch (Exception ex)
                {

                }
            }
        }

        static async Task DequeueByEventIdAsync(string eventId)
        {
            var sql = @"update l set l.[State] = 1
                        output inserted.*
                        from dbo.IntegrationEventLog as l with(rowlock, readpast)
                        where l.EventId = @EventId
                        and l.[State] = 0";

            using (var connection = new SqlConnection(ConnectionString))
            {
                //connection.Open();
                await connection.OpenAsync();

                try
                {
                    //var rowsAffected = connection.Execute(sql, eventLogEntry);
                    var entry = await connection.QueryFirstOrDefaultAsync<IntegrationEventLogEntry>(sql, new { EventId = Guid.Parse(eventId) });
                }
                catch (Exception ex)
                {

                }
            }
        }

        static async Task SelectByEventIdAsync(string eventId)
        {
            var sql = "select l.* from dbo.IntegrationEventLog as l where l.EventId = @EventId";

            using (var connection = new SqlConnection(ConnectionString))
            {
                //connection.Open();
                await connection.OpenAsync();

                try
                {
                    //var rowsAffected = connection.Execute(sql, eventLogEntry);
                    var entry = await connection.QueryFirstOrDefaultAsync<IntegrationEventLogEntry>(sql, new { EventId = Guid.Parse(eventId) });
                }
                catch (Exception ex)
                {

                }
            }

        }

        static async Task EnqueueAsync()
        {
            var eventLogEntry = new IntegrationEventLogEntry(Guid.NewGuid(), "event-name", "content");

            string sql = @"INSERT INTO [dbo].[IntegrationEventLog] ([EventId],[Content]
               ,[CreationTime]
               ,[EventTypeName]
               ,[State]
               ,[TimesSent]) VALUES
           (@EventId
           ,@Content
           ,@CreationTime
           ,@EventTypeName
           ,@State
           ,@TimesSent)";

            using (var connection = new SqlConnection(ConnectionString))
            {
                //connection.Open();
                await connection.OpenAsync();

                try
                {
                    //var rowsAffected = connection.Execute(sql, eventLogEntry);
                    var rowsAffected = await connection.ExecuteAsync(sql, eventLogEntry);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}

using Axerrio.CQRS.API.Application.Query;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.MongoDB
{
    public class SalesMongoDbContext
    {
        private const string _connectionString = @"mongodb://wwimporters:2BEGfcQYFGxvE4Q3RZeXQy5BOUwVjV4UFw1dunfsrzkGHwURBisiI6vawx188Rl6gmbTXcS7Nm1glZqITRKWmg==@wwimporters.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
        private const string _databaseName = "Sales";

        private readonly IMongoDatabase _database = null;

        public SalesMongoDbContext()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);

            if (mongoClient != null)
            {
                _database = mongoClient.GetDatabase(_databaseName);
            }
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<Customer> Customers
        {
            get
            {
                return _database.GetCollection<Customer>("Customers");
            }
        }

        public IMongoCollection<SalesOrder> SalesOrders
        {
            get
            {
                return _database.GetCollection<SalesOrder>("Orders");
            }
        }
    }
}
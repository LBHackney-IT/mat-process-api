using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mat_process_api.V1.Infrastructure
{
    public class MatDbContext : IMatDbContext
    {
        private MongoClient mongoClient;
        private IMongoDatabase mongoDatabase;
        public IMongoCollection<BsonDocument> matProcessCollection { get; set; }
        public MatDbContext(IOptions<ConnectionSettings> appSettings)
        {
            mongoClient = new MongoClient(new MongoUrl(appSettings.Value.ConnectionString));
            //create a new blank database if database does not exist, otherwise get existing database
            mongoDatabase = mongoClient.GetDatabase(appSettings.Value.Database);
            //create collection to hold the documents if it does not exist, otherwise retrieve existing
            matProcessCollection = mongoDatabase.GetCollection<BsonDocument>(appSettings.Value.CollectionName);
        }
        public IMongoCollection<BsonDocument> getCollection()
        {
            return matProcessCollection;
        }
    }
}

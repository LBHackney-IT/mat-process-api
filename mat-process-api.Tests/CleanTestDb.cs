using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using mat_process_api.V1.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UnitTests
{
    public class DbTest
    {
        protected IMongoDatabase mongoDatabase;
        protected IMongoCollection<BsonDocument> collection;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        { 
            string MONGO_CONN_STRING = Environment.GetEnvironmentVariable("MONGO_CONN_STRING") ??
                                 @"mongodb://localhost:1433/";
         
            //connect to local mongo DB
            MongoClient mongoClient = new MongoClient(new MongoUrl(MONGO_CONN_STRING));
            //create a new blank database if database does not exist, otherwise get existing database
            mongoDatabase = mongoClient.GetDatabase("mat-processes");
            //create collection to hold the documents if it does not exist, otherwise retrieve existing
            collection = mongoDatabase.GetCollection<BsonDocument>("process-data");
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
           //clear collection - remove any documents inserted during thet test
           collection.DeleteMany(Builders<BsonDocument>.Filter.Empty);
        }
    }
}

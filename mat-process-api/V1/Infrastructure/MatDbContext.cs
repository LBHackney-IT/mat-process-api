using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            string pathToCAFile = "/tmp/rds-combined-ca-bundle.pem";

            // ADD CA certificate to local trust store
            // DO this once - Maybe when your service starts
            X509Store localTrustStore = new X509Store(StoreName.Root);
            string caContentString = System.IO.File.ReadAllText(pathToCAFile);

            X509Certificate2 caCert = new X509Certificate2(Encoding.ASCII.GetBytes(caContentString));
            try
            {
                localTrustStore.Open(OpenFlags.ReadWrite);
                localTrustStore.Add(caCert);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Root certificate import failed: " + ex.Message);
                throw;
            }
            finally
            {
                localTrustStore.Close();
            }
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

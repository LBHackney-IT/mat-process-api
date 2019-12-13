using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnitTests;

namespace mat_process_api.Tests.V1.Infrastructure
{
    [TestFixture]
    public class MatDbContextTests : DbTest
    {
        private Mock<IOptions<ConnectionSettings>> mockOptions;
        [SetUp]
        public void set_up()
        {
            mockOptions = new Mock<IOptions<ConnectionSettings>>();
           //runtime DB connection strings
           var settings = new ConnectionSettings
           {
               ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONN_STRING") ??
                                  @"mongodb://localhost:1433/",
               CollectionName = "process-data",
               Database = "mat-processes"
           };
            mockOptions.SetupGet(x => x.Value).Returns(settings);        
        }
        [Test]
        public void context_can_get_process_data()
        {
            //arrange
            MatProcessData processData = MatProcessDataHelper.CreateProcessDataObject();
            var bsonObject = BsonDocument.Parse(JsonConvert.SerializeObject(processData));
            collection.InsertOne(bsonObject);
            //act
            var context = new MatDbContext(mockOptions.Object);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", processData.Id.ToString());
            var result = context.getCollection().FindAsync(filter).Result.FirstOrDefault();
            var resultDesirialzie = BsonSerializer.Deserialize<MatProcessData>(result);
            //assert
            Assert.AreEqual(processData.Id, resultDesirialzie.Id);
            Assert.AreEqual(processData.ProcessType.value, resultDesirialzie.ProcessType.value);
            Assert.AreEqual(processData.ProcessType.name, resultDesirialzie.ProcessType.name);
            Assert.AreEqual(processData.DateCreated, resultDesirialzie.DateCreated);
            Assert.AreEqual(processData.DateLastModified, resultDesirialzie.DateLastModified);
            Assert.AreEqual(processData.DateCompleted, resultDesirialzie.DateCompleted);
            Assert.AreEqual(processData.ProcessDataAvailable, resultDesirialzie.ProcessDataAvailable);
            Assert.AreEqual(processData.ProcessDataSchemaVersion, resultDesirialzie.ProcessDataSchemaVersion);
            Assert.AreEqual(processData.ProcessStage, resultDesirialzie.ProcessStage);
            Assert.AreEqual(processData.LinkedProcessId, resultDesirialzie.LinkedProcessId);
            Assert.AreEqual(processData.PreProcessData, resultDesirialzie.PreProcessData);
            Assert.AreEqual(processData.ProcessData, resultDesirialzie.ProcessData);
            Assert.AreEqual(processData.PostProcessData, resultDesirialzie.PostProcessData);
        }
    }
}

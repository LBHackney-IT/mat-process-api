using System;
using System.Linq;
using Bogus;
using mat_process_api.Tests.V1.Helper;
using NUnit.Framework;
using mat_process_api.V1.Domain;
using UnitTests.V1.Helper;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Infrastructure;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace UnitTests.V1.Gateways
{
    [TestFixture]
    public class ProcessesGatewayTests : DbTest { 

        private readonly Faker _faker = new Faker();
        private ProcessDataGateway processDataGateway;
        private Mock<IMatDbContext> mockContext;

        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<IMatDbContext>();
            mockContext.Setup(x => x.getCollection()).Returns(collection);
            processDataGateway = new ProcessDataGateway(mockContext.Object);
        }

        [Test]
        public void test_that_gateway_class_implements_gateway_interface()
        {
            Assert.NotNull(processDataGateway is IProcessDataGateway);
        }

        [Test]
        public void test_that_gateway_return_object_matches_object_in_database()
        {
            //arrange
            MatProcessData processData = MatProcessDataHelper.CreateProcessDataObject();
            var bsonObject = BsonDocument.Parse(JsonConvert.SerializeObject(processData));
            collection.InsertOne(bsonObject);
            //act
            var result = processDataGateway.GetProcessData(processData.Id);
            //assert
            Assert.AreEqual(result.Id, processData.Id);
            Assert.AreEqual(result.DateLastModified, processData.DateLastModified);
            Assert.AreEqual(result.ProcessType, processData.ProcessType);
            Assert.AreEqual(result.ProcessStage, processData.ProcessStage);
            Assert.AreEqual(result.PreProcessData, processData.PreProcessData);
            Assert.AreEqual(result.ProcessData, processData.ProcessData);
            Assert.AreEqual(result.PostProcessData, processData.PostProcessData);
            Assert.AreEqual(result.DateCreated, processData.DateCreated);
            Assert.AreEqual(result.DateCompleted, processData.DateCompleted);
            Assert.AreEqual(result.DataSchemaVersion,processData.DataSchemaVersion);
            Assert.IsInstanceOf<MatProcessData>(result);
        }


        [Test]
        public void test_that_gateway_return_mat_process_object_if_no_match_is_found()
        {
            //arrange
            var id = _faker.Random.Word();
            //act
            var result = processDataGateway.GetProcessData(id);
            //assert
            Assert.AreEqual(result.Id, null);
            Assert.AreEqual(result.DateLastModified, DateTime.MinValue);
            Assert.AreEqual(result.ProcessType, null);
            Assert.AreEqual(result.ProcessStage, null);
            Assert.AreEqual(result.PreProcessData,null);
            Assert.AreEqual(result.ProcessData, null);
            Assert.AreEqual(result.PostProcessData, null);
            Assert.AreEqual(result.DateCreated, DateTime.MinValue);
            Assert.AreEqual(result.DateCompleted, DateTime.MinValue);
            Assert.AreEqual(result.DataSchemaVersion, 0);
            Assert.IsInstanceOf<MatProcessData>(result);
        }

        [Test]
        public void dummy_example_test_tmongo()
        {
            //leave test temporarily for reference
            JObject test = new JObject();
            test.Add("_id",_faker.Random.Guid());
            test.Add("processType", _faker.Random.Word());

            //parse to bson
            var update = BsonDocument.Parse(test.ToString());
            //insert into db
            collection.InsertOne(update);

            //ensure we have inserted document
            Assert.AreEqual(collection.CountDocuments(Builders<BsonDocument>.Filter.Empty),1);
        }
    }
}

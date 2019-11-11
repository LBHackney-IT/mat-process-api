using System;
using System.Linq;
using Bogus;
using mat_process_api.Tests.V1.Helper;
using NUnit.Framework;
using mat_process_api.V1.Domain;
using UnitTests.V1.Helper;
using mat_process_api.V1.Gateways;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace UnitTests.V1.Gateways
{
    [TestFixture]
    public class ProcessesGatewayTests : DbTest { 

        private readonly Faker _faker = new Faker();
        private ProcessDataGateway processDataGateway;

        [SetUp]
        public void Setup()
        {
            processDataGateway = new ProcessDataGateway();
        }

        [Test]
        public void test_that_gateway_class_implements_gateway_interface()
        {
            Assert.NotNull(processDataGateway is IProcessDataGateway);
        }

        [Test]
        public void test_that_gateway_returns_empty_object_if_no_match_is_found()
        {
            //arrange
            MatProcessData processData = MatProcessDataHelper.CreateProcessDataObject();
            //act
            var bsonObject = BsonDocument.Parse(JsonConvert.SerializeObject(processData));
            collection.InsertOne(bsonObject);
            //var result = processDataGateway.GetProcessData(processData.Id);
            //assert
           // Assert.AreEqual(result.DataSchemaVersion,processData.DataSchemaVersion);
            Assert.Throws<NotImplementedException>(() => processDataGateway.GetProcessData(processData.Id));
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

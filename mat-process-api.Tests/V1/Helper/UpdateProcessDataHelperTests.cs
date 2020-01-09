using Bogus;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace mat_process_api.Tests.V1.Helper
{
    [TestFixture]
    public class UpdateProcessDataHelperTests
{
        private Faker _faker;
        [SetUp]
        public void setup()
        {
            _faker = new Faker();
        }
        [Test]
        public void can_check_for_fields_to_be_updated_from_a_request_object_and_prepare_update_definition()
        {
            //arrange
            var matProcessData = new MatUpdateProcessData();
            //add fields to be updated
            matProcessData.ProcessData = new
            {
                firstField = _faker.Random.Word(),
                anyField = _faker.Random.Words(),
                numberField = _faker.Random.Number()
            };
            matProcessData.DateLastModified = _faker.Date.Recent();
            var expectedUpdateDefinition = Builders<BsonDocument>.Update.Combine(
                Builders<BsonDocument>.Update.Set("dateLastModified", matProcessData.DateLastModified),
                Builders<BsonDocument>.Update.Set("processDataAvailable", true),
                Builders<BsonDocument>.Update.Set("processData", BsonDocument.Parse(JsonConvert.SerializeObject(matProcessData.ProcessData))));
            //act
            UpdateDefinition<BsonDocument> processDataUpdateDefinition = UpdateProcessDocumentHelper.PrepareFieldsToBeUpdated(matProcessData);
            //assert
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            //compare what the update definitions render to
            Assert.AreEqual(expectedUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString(),
                processDataUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString());
        }

        [TestCase("test")]
        [TestCase("test-123--")]
        [TestCase("4455")]
        public void can_check_for_fields_to_be_updated_from_a_request_object_correctly(string firstField)
        {
            //this test ensures that the update definition is built correctly and will be recognised as different when compared to a different
            //update definition
            //arrange
            var matProcessData = new MatUpdateProcessData();
            //add fields to be updated
            matProcessData.ProcessData = new
            {
                firstField = _faker.Random.Word(),
                anyField = _faker.Random.Words(),
                numberField = _faker.Random.Number()
            };

            matProcessData.DateLastModified = _faker.Date.Recent();

            var expectedUpdateDefinition = Builders<BsonDocument>.Update.Combine(
                Builders<BsonDocument>.Update.Set("dateLastModified", matProcessData.DateLastModified),
                  Builders<BsonDocument>.Update.Set("processDataa", matProcessData.ProcessData));
            //act
            UpdateDefinition<BsonDocument> processDataUpdateDefinition = UpdateProcessDocumentHelper.PrepareFieldsToBeUpdated(matProcessData);
            //assert
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            //test that update definitions are succesfully recognised as different
            Assert.AreNotEqual(expectedUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString(),
                processDataUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString());
        }
       
    }
}

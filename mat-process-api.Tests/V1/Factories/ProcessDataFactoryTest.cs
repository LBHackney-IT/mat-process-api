using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using System.Reflection;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Boundary;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using JsonConvert = Newtonsoft.Json.JsonConvert;


namespace mat_process_api.Tests.V1.Factories
{
    [TestFixture]
    public class ProcessDataFactoryTest
    {
        private Faker _faker;
        [SetUp]
        public void setup()
        {
            _faker = new Faker();
        }
        [Test]
        public void can_create_mat_process_data_object_from_empty_object()
        {
            //arrange
            var matProcessData = new MatProcessData();
            //act
            var processDataFromFactory = ProcessDataFactory.CreateProcessDataObject(BsonDocument.Parse(JsonConvert.SerializeObject(matProcessData)));
            //assert
            Assert.AreEqual(matProcessData.ProcessDataSchemaVersion,processDataFromFactory.ProcessDataSchemaVersion);
            Assert.AreEqual(matProcessData.DateCompleted, processDataFromFactory.DateCompleted);
            Assert.AreEqual(matProcessData.DateCreated, processDataFromFactory.DateCreated);
            Assert.AreEqual(matProcessData.DateLastModified, processDataFromFactory.DateLastModified);
            Assert.AreEqual(matProcessData.Id, processDataFromFactory.Id);
            Assert.AreEqual(matProcessData.PostProcessData, processDataFromFactory.PostProcessData);
            Assert.AreEqual(matProcessData.PreProcessData, processDataFromFactory.PreProcessData);
            Assert.AreEqual(matProcessData.ProcessData, processDataFromFactory.ProcessData);
            Assert.AreEqual(matProcessData.ProcessStage, processDataFromFactory.ProcessStage);
            Assert.AreEqual(matProcessData.ProcessType, processDataFromFactory.ProcessType);
        }

        [Test]
        public void can_create_mat_process_data_object_from_populated_object()
        {
            //arrange
            var matProcessData = MatProcessDataHelper.CreateProcessDataObject();
            //act
            var processDataFromFactory = ProcessDataFactory.CreateProcessDataObject(BsonDocument.Parse(JsonConvert.SerializeObject(matProcessData)));
            //assert
            Assert.AreEqual(matProcessData.Id, processDataFromFactory.Id);
            Assert.AreEqual(matProcessData.ProcessType.value, processDataFromFactory.ProcessType.value);
            Assert.AreEqual(matProcessData.ProcessType.name, processDataFromFactory.ProcessType.name);
            Assert.AreEqual(matProcessData.DateCreated, processDataFromFactory.DateCreated);
            Assert.AreEqual(matProcessData.DateLastModified, processDataFromFactory.DateLastModified);
            Assert.AreEqual(matProcessData.DateCompleted, processDataFromFactory.DateCompleted);
            Assert.AreEqual(matProcessData.ProcessDataAvailable, processDataFromFactory.ProcessDataAvailable);
            Assert.AreEqual(matProcessData.ProcessDataSchemaVersion, processDataFromFactory.ProcessDataSchemaVersion);
            Assert.AreEqual(matProcessData.ProcessStage, processDataFromFactory.ProcessStage);
            Assert.AreEqual(matProcessData.LinkedProcessId, processDataFromFactory.LinkedProcessId);
            Assert.AreEqual(matProcessData.PreProcessData, processDataFromFactory.PreProcessData);
            Assert.AreEqual(matProcessData.ProcessData, processDataFromFactory.ProcessData);
            Assert.AreEqual(matProcessData.PostProcessData, processDataFromFactory.PostProcessData);
        }

        [Test]
        public void given_postInitialProcessDocumentRequest_object_when_createProcessDataObject_factory_method_is_called_it_returns_correctly_populated_processData_domain_object()
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();

            //act
            var domainObject = ProcessDataFactory.CreateProcessDataObject(requestObject);

            //assert
            Assert.NotNull(domainObject);
            Assert.IsInstanceOf<MatProcessData>(domainObject);
            Assert.AreEqual(requestObject.processRef, domainObject.Id);
            Assert.AreEqual(requestObject.processType.value, domainObject.ProcessType.value);
            Assert.AreEqual(requestObject.processType.name, domainObject.ProcessType.name);

            Assert.NotNull(domainObject.DateCreated);
            Assert.NotNull(domainObject.DateLastModified);
            Assert.AreEqual(domainObject.DateCreated, domainObject.DateLastModified); //DateLastModified should be equal to the date of creation because the test is for an object that is created now.
            Assert.AreEqual(DateTime.MinValue, domainObject.DateCompleted);

            Assert.False(domainObject.ProcessDataAvailable);
            Assert.AreEqual(requestObject.processDataSchemaVersion, domainObject.ProcessDataSchemaVersion);
            Assert.AreEqual("Not completed", domainObject.ProcessStage); //it's not confirmed yet whether it's going to be int or string
            Assert.Null(domainObject.LinkedProcessId);

            // Assert.AreEqual(new { }, domainObject.PreProcessData); //Causes error --> Expected: <{ }> (<>f__AnonymousType0); But was:  <{ }> (<> f__AnonymousType0)
            Assert.AreEqual(0, domainObject.PreProcessData.GetType().GetProperties().Count()); // using reflections, because the above won't work
            Assert.AreEqual(0, domainObject.ProcessData.GetType().GetProperties().Count());
            Assert.AreEqual(0, domainObject.PostProcessData.GetType().GetProperties().Count());
        }

        #region Update process data
        [Test]
        public void can_check_for_fields_to_be_updated_from_a_request_object_and_prepare_update_definition()
        {
            //arrange
            var matProcessData = new MatProcessData();
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
                  Builders<BsonDocument>.Update.Set("processData", matProcessData.ProcessData));
            //act
            UpdateDefinition<BsonDocument> processDataUpdateDefinition = ProcessDataFactory.PrepareFieldsToBeUpdated(matProcessData);
            //assert
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            //compare what the update definitions render to
            Assert.AreEqual(expectedUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString(),
                processDataUpdateDefinition.Render(documentSerializer,serializerRegistry).ToString());
        }

        [TestCase("test")]
        [TestCase("test-123--")]
        [TestCase("4455")]
        public void can_check_for_fields_to_be_updated_from_a_request_object_correctly(string firstField)
        {
            //this test ensures that the update definition is built correctly and will be recognised as different when compared to a different
            //update definition
            //arrange
            var matProcessData = new MatProcessData();
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
            UpdateDefinition<BsonDocument> processDataUpdateDefinition = ProcessDataFactory.PrepareFieldsToBeUpdated(matProcessData);
            //assert
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<BsonDocument>();
            //test that update definitions are succesfully recognised as different
            Assert.AreNotEqual(expectedUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString(),
                processDataUpdateDefinition.Render(documentSerializer, serializerRegistry).ToString());
        }
        #endregion
    }
}

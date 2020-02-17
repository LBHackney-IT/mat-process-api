using System;
using System.Linq;
using Bogus;
using mat_process_api.Tests.V1.Helper;
using NUnit.Framework;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Infrastructure;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Boundary;
using MongoDB.Bson.Serialization;
using mat_process_api.V1.Exceptions;
using mat_process_api.V1.Helpers;

namespace UnitTests.V1.Gateways
{
    [TestFixture]
    public class ProcessesGatewayTests : DbTest { 

        private readonly Faker _faker = new Faker();
        private ProcessDataGateway processDataGateway;
        private Mock<IMatDbContext> mockContext;

        [SetUp]
        public void set_up()
        {           
            mockContext = new Mock<IMatDbContext>();
            mockContext.Setup(x => x.getCollection()).Returns(collection);
            processDataGateway = new ProcessDataGateway(mockContext.Object);
        }

        #region Get Process Document

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
            Assert.AreEqual(processData.Id, result.Id);
            Assert.AreEqual(processData.ProcessType.value, result.ProcessType.value);
            Assert.AreEqual(processData.ProcessType.name, result.ProcessType.name);
            Assert.AreEqual(processData.DateCreated, result.DateCreated);
            Assert.AreEqual(processData.DateLastModified, result.DateLastModified);
            Assert.AreEqual(processData.DateCompleted, result.DateCompleted);
            Assert.AreEqual(processData.ProcessDataAvailable, result.ProcessDataAvailable);
            Assert.AreEqual(processData.ProcessDataSchemaVersion, result.ProcessDataSchemaVersion);
            Assert.AreEqual(processData.ProcessStage, result.ProcessStage);
            Assert.AreEqual(processData.LinkedProcessId, result.LinkedProcessId);
            Assert.AreEqual(processData.PreProcessData, result.PreProcessData);
            Assert.AreEqual(processData.ProcessData, result.ProcessData);
            Assert.AreEqual(processData.PostProcessData, result.PostProcessData);
            Assert.IsInstanceOf<MatProcessData>(result);
        }

        [Test]
        public void given_nonexistent_processRef_when_GetProcessData_gateway_method_is_called_then_gateway_throws_DocumentNotFound_exception()
        {
            //arrange
            string processRef = _faker.Random.Guid().ToString();

            //act, assert
            Assert.Throws<DocumentNotFound>(() => processDataGateway.GetProcessData(processRef));
        }

        #endregion

        #region Update Process
        [Test]
        public void test_that_object_can_be_successfully_updated()
        {
            //arrange
            MatProcessData processData = MatProcessDataHelper.CreateProcessDataObject();
            var bsonObject = BsonDocument.Parse(JsonConvert.SerializeObject(processData));
            collection.InsertOne(bsonObject);
            //object to update
            var objectToUpdate = new MatUpdateProcessData();
            var processRef = processData.Id;
            objectToUpdate.DateLastModified = _faker.Date.Recent();
            objectToUpdate.ProcessData = new
            {
                firstField = _faker.Random.Word(),
                anyField = _faker.Random.Words(),
                numberField = _faker.Random.Number()
            };
            //get update definition
            var updateDefinition = UpdateProcessDocumentHelper.PrepareFieldsToBeUpdated(objectToUpdate);

            //act
            var result = processDataGateway.UpdateProcessData(updateDefinition,processRef);
            //assert
            Assert.AreEqual(processRef, result.Id);
            Assert.AreEqual(JsonConvert.SerializeObject(objectToUpdate.ProcessData), JsonConvert.SerializeObject(result.ProcessData));
            Assert.AreEqual(objectToUpdate.DateLastModified.ToShortDateString(), result.DateLastModified.ToShortDateString());
            Assert.IsInstanceOf<MatProcessData>(result);
        }
        [Test]
        public void test_if_object_to_be_updated_is_not_found_exception_is_thrown()
        {
            //arrange
            //object to update
            var objectToUpdate = new MatUpdateProcessData();
            objectToUpdate.DateLastModified = _faker.Date.Recent();
            objectToUpdate.ProcessData = new
            {
                firstField = _faker.Random.Word(),
                anyField = _faker.Random.Words(),
                numberField = _faker.Random.Number()
            };
            var processRef = _faker.Random.Guid().ToString();
            //get update definition
            var updateDefinition = UpdateProcessDocumentHelper.PrepareFieldsToBeUpdated(objectToUpdate);
            //assert
            Assert.Throws<DocumentNotFound>(() => processDataGateway.UpdateProcessData(updateDefinition, processRef));
        }
        #endregion
        #region Post Initial Process Document

        [Test]
        public void given_the_matProcessData_domain_object_when_postInitialProcessDocument_gateway_method_is_called_then_the_parsed_object_gets_added_into_the_database()
        {
            //arrange
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            processDataGateway.PostInitialProcessDocument(domainObject);

            //assert
            var documentFromDB = BsonSerializer.Deserialize<MatProcessData>(collection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", domainObject.Id)).Result.FirstOrDefault());

            Assert.AreEqual(domainObject.Id, documentFromDB.Id);
            Assert.AreEqual(domainObject.ProcessType.value, documentFromDB.ProcessType.value);
            Assert.AreEqual(domainObject.ProcessType.name, documentFromDB.ProcessType.name);
            Assert.AreEqual(domainObject.ProcessDataSchemaVersion, documentFromDB.ProcessDataSchemaVersion);
        }

        [Test]
        public void given_the_matProcessData_domain_object_when_postInitialProcessDocument_gateway_method_is_called_then_the_number_of_documents_in_the_database_increases_by_one() //test that checks whether the db doesn't get cleared or overwritten somehow upon insertion
        {
            //arrange
            var unclearedDocumentCount = collection.CountDocuments(Builders<BsonDocument>.Filter.Empty); //did some testing around this, seems like the database doesn't get cleared after every test. Depending on the ordering, it might not actually be empty at the start of this test. When this is unaccounted for, it makes this test fail.

            //pre-insert between 0 and 7 documents into database, so that it wouldn't be necessarily empty (triangulation)
            int preInsertedDocumentCount = _faker.Random.Int(0, 7);
            for (int i = preInsertedDocumentCount; i > 0; i--)
            {
                MatProcessData preInsertedDomainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());
                collection.InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(preInsertedDomainObject)));
            }

            //a new object that will be inserted upon gateway call
            MatProcessData toBeInsertedDomainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            processDataGateway.PostInitialProcessDocument(toBeInsertedDomainObject);

            //assert
            var startingDocumentCount = unclearedDocumentCount + preInsertedDocumentCount;
            Assert.AreEqual(startingDocumentCount + 1, collection.CountDocuments(Builders<BsonDocument>.Filter.Empty));
        }

        [Test]
        public void given_the_matProcessData_domain_object_when_postInitialProcessDocument_gateway_method_is_called_then_it_returns_id_of_the_inserted_document()
        {
            //arrange
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            string response_Id = processDataGateway.PostInitialProcessDocument(domainObject);

            //assert
            Assert.AreEqual(domainObject.Id, response_Id);
        }

        [Test]
        public void given_the_double_insert_of_matProcessData_domain_object_when_postInitialProcessDocument_gateway_method_is_called_then_conflict_exception_is_thrown()
        {
            //arrange
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            string response_Id = processDataGateway.PostInitialProcessDocument(domainObject); //first insert.

            Assert.Throws<ConflictException>(() => processDataGateway.PostInitialProcessDocument(domainObject)); //the second document insertion happends, while asserting.
        }
        #endregion

    }
}

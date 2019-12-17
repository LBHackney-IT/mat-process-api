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
using mat_process_api.V1.Factories;
using mat_process_api.V1.Boundary;
using MongoDB.Bson.Serialization;

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
        public void test_that_gateway_return_mat_process_object_if_no_match_is_found()
        {
            //arrange
            string id = _faker.Random.Guid().ToString();
            //act
            var result = processDataGateway.GetProcessData(id);
            //assert
            Assert.Null(result.Id);
            Assert.Null(result.ProcessType);
            Assert.AreEqual(DateTime.MinValue, result.DateCreated);
            Assert.AreEqual(DateTime.MinValue, result.DateLastModified);
            Assert.AreEqual(DateTime.MinValue, result.DateCompleted);
            Assert.False(result.ProcessDataAvailable);
            Assert.Zero(result.ProcessDataSchemaVersion);
            Assert.Null(result.ProcessStage);
            Assert.Null(result.LinkedProcessId);
            Assert.Null(result.PreProcessData);
            Assert.Null(result.ProcessData);
            Assert.Null(result.PostProcessData);
            Assert.IsInstanceOf<MatProcessData>(result);
        }

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
            int toBeInsertedDocumentCount = _faker.Random.Int(0, 7);
            for (int i = toBeInsertedDocumentCount; i > 0; i--)
            {
                MatProcessData preInsertedDomainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());
                collection.InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(preInsertedDomainObject)));
            }

            //a new object that will be inserted upon gateway call
            MatProcessData toBeInsertedDomainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            processDataGateway.PostInitialProcessDocument(toBeInsertedDomainObject);

            //assert
            var startingDocumentCount = unclearedDocumentCount + toBeInsertedDocumentCount;
            Assert.AreEqual(startingDocumentCount + 1, collection.CountDocuments(Builders<BsonDocument>.Filter.Empty));
        }

        [Test]
        public void given_the_matProcessData_domain_object_when_postInitialProcessDocument_gateway_method_is_called_then_it_returns_id_of_the_inserted_document() //test that checks whether the db doesn't get cleared or overwritten somehow upon insertion
        {
            //arrange
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            string response_Id = processDataGateway.PostInitialProcessDocument(domainObject);

            //assert
            Assert.AreEqual(domainObject.Id, response_Id);
        }

        #endregion
    }
}

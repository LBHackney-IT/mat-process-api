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
            Assert.AreEqual(result.Id, processData.Id);
            Assert.AreEqual(result.DateLastModified, processData.DateLastModified);
            Assert.AreEqual(result.ProcessType, processData.ProcessType);
            Assert.AreEqual(result.ProcessStage, processData.ProcessStage);
            Assert.AreEqual(result.PreProcessData, processData.PreProcessData);
            Assert.AreEqual(result.ProcessData, processData.ProcessData);
            Assert.AreEqual(result.PostProcessData, processData.PostProcessData);
            Assert.AreEqual(result.DateCreated, processData.DateCreated);
            Assert.AreEqual(result.DateCompleted, processData.DateCompleted);
            Assert.AreEqual(result.ProcessDataSchemaVersion,processData.ProcessDataSchemaVersion);
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
            Assert.AreEqual(result.ProcessDataSchemaVersion, 0);
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
            Assert.AreEqual(domainObject.ProcessType, documentFromDB.ProcessType);
            Assert.AreEqual(domainObject.ProcessDataSchemaVersion, documentFromDB.ProcessDataSchemaVersion);
        }

        [Test]
        public void given_the_matProcessData_domain_object_when_postInitialProcessDocument_gateway_method_is_called_then_the_number_of_documents_in_the_database_increases_by_one() //test that checks whether the db doesn't get cleared or overwritten somehow upon insertion
        {
            //arrange
            //pre-insert between 0 and 7 documents into database, so that it wouldn't be necessarily empty (triangulation)
            int startingDocumentCount = _faker.Random.Int(0, 7);
            for (int i = startingDocumentCount; i > 0; i--)
            {
                MatProcessData preInsertedDomainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());
                collection.InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(preInsertedDomainObject)));
            }

            //a new object that will be inserted upon gateway call
            MatProcessData toBeInsertedDomainObject = ProcessDataFactory.CreateProcessDataObject(MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject());

            //act
            processDataGateway.PostInitialProcessDocument(toBeInsertedDomainObject);

            //assert
            Assert.AreEqual(startingDocumentCount + 1, collection.CountDocuments(Builders<BsonDocument>.Filter.Empty));
        }

        #endregion
    }
}

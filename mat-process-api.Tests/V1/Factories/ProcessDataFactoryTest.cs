using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Boundary;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using NUnit.Framework;
using JsonConvert = Newtonsoft.Json.JsonConvert;


namespace mat_process_api.Tests.V1.Factories
{
    [TestFixture]
    public class ProcessDataFactoryTest
    {
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
    }
}

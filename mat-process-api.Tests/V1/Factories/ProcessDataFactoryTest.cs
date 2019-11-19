using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
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
            Assert.AreEqual(matProcessData.ProcessDataSchemaVersion, processDataFromFactory.ProcessDataSchemaVersion);
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
    }
}

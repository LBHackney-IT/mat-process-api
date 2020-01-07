using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.UseCase;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace mat_process_api.Tests.V1.UseCase
{
    [TestFixture]
    public class ProcessDataUseCaseTests
    {
        private Faker faker;
        private ProcessDataUseCase processDataUseCase;
        private Mock<IProcessDataGateway> mockMatGateway;
        [SetUp]
        public void set_up()
        {
            faker = new Faker();
            mockMatGateway = new Mock<IProcessDataGateway>();
            processDataUseCase = new ProcessDataUseCase(mockMatGateway.Object);
        }
        [Test]
        public void get_process_data_by_processref_returns_getprocessdataresponse_object()
        {
            //arrange
            var processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest { processRef = processRef };
            var response = new MatProcessData();
            //act        
            mockMatGateway.Setup(x => x.GetProcessData(processRef)).Returns(response);
            var result = processDataUseCase.ExecuteGet(request);
            //assert
            Assert.IsInstanceOf<GetProcessDataResponse>(result);
            Assert.IsInstanceOf<GetProcessDataRequest>(result.Request);
            Assert.IsInstanceOf<MatProcessData>(result.ProcessData);
            Assert.AreEqual(processRef, result.Request.processRef);
            Assert.NotNull(result.GeneratedAt);
            Assert.IsInstanceOf<DateTime>(result.GeneratedAt);
        }

        [Test]
        public void test_gateway_is_called()
        {
            //arrange
            var processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest{processRef = processRef};
            //act
            processDataUseCase.ExecuteGet(request);
            mockMatGateway.Verify(x => x.GetProcessData(processRef));
        }

        [TestCase("123")]
        [TestCase("acb5")]
        [TestCase("aa-bb-123")]
        public void get_process_data_verify_gateway_calls_database_with_parameters(string processRef)
        {
            //arrange            
            var request = new GetProcessDataRequest { processRef = processRef };
            //act
            var result = processDataUseCase.ExecuteGet(request);
            //assert
            mockMatGateway.Verify(v => v.GetProcessData(It.Is<string>(i => i == processRef)), Times.Once);
        }

        [Test]
        public void check_gateway_returns_expected_response()
        {
            //arrange
            var processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest { processRef = processRef };
            var response = new MatProcessData();
            //act
            mockMatGateway.Setup(x => x.GetProcessData(processRef)).Returns(response);
            var result = processDataUseCase.ExecuteGet(request);
            //assert
            Assert.AreEqual(response, result.ProcessData);
        }

        #region Update Process Data
        [Test]
        public void update_process_data_verify_gateway_calls_database_with_parameters()
        {
            //arrange            
            var request = new UpdateProcessDataRequest() { processDataToUpdate = new MatProcessData() };
            //act
            var result = processDataUseCase.ExecuteUpdate(request);
            //assert
            mockMatGateway.Verify(v => v.UpdateProcessData(It.IsAny<UpdateDefinition<BsonDocument>>(),It.IsAny<string>()), Times.Once);
        }
        [Test]
        public void update_process_data_gateway_call_returns_updateprocsesdataresponse_object()
        {
            //arrange
            var updateData = new MatProcessData();
            var request = new UpdateProcessDataRequest() { processDataToUpdate = updateData };

            //act
            mockMatGateway.Setup(x => x.UpdateProcessData(It.IsAny<UpdateDefinition<BsonDocument>>(),It.IsAny<string>())).Returns(updateData);
            var result = processDataUseCase.ExecuteUpdate(request);
            //assert
            Assert.IsInstanceOf<MatProcessData>(result.ProcessData);
            Assert.AreEqual(updateData, result.ProcessData);
        }

        [Test]
        public void update_process_data_returns_updateprocessdataresponse_object()
        {
            //arrange
            var processRef = faker.Random.Guid().ToString();
            var request = new UpdateProcessDataRequest { processDataToUpdate = new MatProcessData() };
            var response = new MatProcessData();
            //act        
            mockMatGateway.Setup(x => x.UpdateProcessData(It.IsAny<UpdateDefinition<BsonDocument>>(),It.IsAny<string>())).Returns(response);
            var result = processDataUseCase.ExecuteUpdate(request);
            //assert
            Assert.IsInstanceOf<UpdateProcessDataResponse>(result);
            Assert.IsInstanceOf<UpdateProcessDataRequest>(result.Request);
            Assert.IsInstanceOf<MatProcessData>(result.ProcessData);
            Assert.AreEqual(JsonConvert.SerializeObject(new MatProcessData()), JsonConvert.SerializeObject(result.ProcessData));
            Assert.NotNull(result.GeneratedAt);
            Assert.IsInstanceOf<DateTime>(result.GeneratedAt);
        }
        [TestCase("123-456-789-000")]
        [TestCase("acb5-abc-abc-abc")]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public void update_process_data_verify_gateway_calls_database_with_parameters(string processRef)
        {
            //arrange
            var dataToUpdate = new MatProcessData();
            dataToUpdate.Id = processRef;
            var request = new UpdateProcessDataRequest { processDataToUpdate = dataToUpdate };
            //act
            var result = processDataUseCase.ExecuteUpdate(request);
            //assert
            mockMatGateway.Verify(v => v.UpdateProcessData(It.IsAny<UpdateDefinition<BsonDocument>>(),It.Is<string>(i => i == processRef)), Times.Once);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.UseCase;
using mat_process_api.V1.Factories;
using mat_process_api.Tests.V1.Helper;
using Microsoft.IdentityModel.Tokens;
using Moq;
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
            string processRef = faker.Random.Guid().ToString();
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
            string processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest{processRef = processRef};
            //act
            processDataUseCase.ExecuteGet(request);
            mockMatGateway.Verify(x => x.GetProcessData(processRef));
        }

        public void verif_gateway_calls_database_with_parameters()
        {
            //arrange
            string processRef = faker.Random.Guid().ToString();
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
            string processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest { processRef = processRef };
            var response = new MatProcessData();
            //act
            mockMatGateway.Setup(x => x.GetProcessData(processRef)).Returns(response);
            var result = processDataUseCase.ExecuteGet(request);
            //assert
            Assert.AreEqual(response, result.ProcessData);
        }

        [Test]
        public void given_the_requestObject_when_executePost_usecase_method_is_called_then_the_gateway_is_called_with_factory_output() // this should be 2 tests really, one to see if factory gets called, and the other to see if gateway is called, but due to using static factory method this becomes impossible to separate.
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(requestObject);

            //act
            processDataUseCase.ExecutePost(requestObject);

            //assert
            mockMatGateway.Verify(g => g.PostInitialProcessDocument(It.Is<MatProcessData>(
                d => d.Id == requestObject.processRef &&
                d.ProcessType.value == requestObject.processType.value &&
                d.ProcessType.name == requestObject.processType.name &&
                d.ProcessDataSchemaVersion == requestObject.processDataSchemaVersion &&
                d.ProcessStage == "0"
                )), Times.Once);

            // I need to include into assertion just enough object properties so that it would be demonstrated that all the data from the request is in a mapped-to object (probably 1 would be enough).
            // and at least one of the properties that factory method generated in order to show that the mapping was done through factory.
            // I don't need to include all the object properties since that's what the Factory method test is for - to test if everything gets mapped and generated as per spec.
        }
    }
}

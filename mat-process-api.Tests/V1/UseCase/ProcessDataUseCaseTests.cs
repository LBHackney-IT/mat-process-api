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

        #region Post Initial Process Document

        //Ideally would have a test for whether the usecase calls factory method, unfortunatelly, there's no way to test that due to factory being a 'static' method, which can't be mocked.

        [Test]
        public void given_the_requestObject_when_executePost_usecase_method_is_called_then_the_gateway_is_called_with_factory_output() // this should be 2 tests really, one to see if factory gets called, and the other to see if gateway is called, but due to using static factory method this becomes impossible to separate.
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(requestObject);
            mockMatGateway.Setup(g => g.PostInitialProcessDocument(It.IsAny<MatProcessData>())).Returns((MatProcessData dobj) => dobj.Id);
            //act
            processDataUseCase.ExecutePost(requestObject);

            //assert
            mockMatGateway.Verify(g => g.PostInitialProcessDocument(It.Is<MatProcessData>(d =>
                d.ProcessType.name == requestObject.processType.name && //checking whether properties that have to be transfered from request object to domain object are present
                d.Id == requestObject.processRef &&
                d.ProcessType.value == requestObject.processType.value &&
                d.ProcessDataSchemaVersion == requestObject.processDataSchemaVersion &&
                d.DateCompleted == DateTime.MinValue && //checking whether some of the properties that factory has to generate itself are present.
                d.ProcessStage == "Not completed"
                )), Times.Once);
            // This checks whether the usecase calls the gateway with the output of the factory method.
        }

        [Test]
        public void given_the_requestObject_when_executePost_usecase_method_is_called_then_it_wraps_gateway_output_into_postInitialDocumentResponse_object()
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            MatProcessData domainObject = ProcessDataFactory.CreateProcessDataObject(requestObject);
            mockMatGateway.Setup(g => g.PostInitialProcessDocument(It.IsAny<MatProcessData>())).Returns((MatProcessData dobj) => dobj.Id);

            //act
            var usecaseResponse = processDataUseCase.ExecutePost(requestObject);

            //assert
            Assert.IsInstanceOf<PostInitialProcessDocumentResponse>(usecaseResponse);

            Assert.IsInstanceOf<PostInitialProcessDocumentRequest>(usecaseResponse.Request);
            Assert.IsInstanceOf<string>(usecaseResponse.ProcessRef);
            Assert.IsInstanceOf<DateTime>(usecaseResponse.GeneratedAt);

            Assert.AreEqual(requestObject.processRef, usecaseResponse.Request.processRef);
            Assert.AreEqual(requestObject.processRef, usecaseResponse.ProcessRef);
            Assert.NotNull(usecaseResponse.GeneratedAt);
            Assert.AreNotEqual(DateTime.MinValue, usecaseResponse.GeneratedAt);
        }

        #endregion
    }
}

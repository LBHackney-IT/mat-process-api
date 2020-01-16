using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Factories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Factories
{
    [TestFixture]
    public class ImageDataFactoryTests
    {
        [Test]
        public void given_a_request_and_decodedBase64Data_objects_when_CreateImageDataObject_factory_method_is_called_then_it_outputs_correctly_populated_ProcessImageData_object()
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            var decodedData = MatProcessDataHelper.CreateBase64DecodedDataObject();

            //act
            var processImageData = ImageDataFactory.CreateImageDataObject(request, decodedData);

            //asssert
            Assert.AreEqual(request.processRef, processImageData.processRef);
            Assert.AreEqual(request.imageId, processImageData.imageId);
            Assert.AreEqual(decodedData.imageType, processImageData.imageData.imageType);
            Assert.AreEqual(decodedData.imageExtension, processImageData.imageData.imageExtension);
            Assert.True(decodedData.imageBytes.SequenceEqual(processImageData.imageData.imageBytes));
        }
    }
}

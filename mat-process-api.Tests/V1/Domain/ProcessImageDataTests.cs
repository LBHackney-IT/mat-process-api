using mat_process_api.V1.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Domain
{
    [TestFixture]
    public class ProcessImageDataTests
    {
        private ProcessImageData _processImageData;
        private Base64DecodedData _base64DecodedData;

        [SetUp]
        public void SetUp()
        {
            //act
            _processImageData = new ProcessImageData();
            _base64DecodedData = new Base64DecodedData();
        }

        #region ProcessImageData

        [Test]
        public void when_a_new_ProcessImageData_domain_object_is_created_its_processRef_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;
            //assert
            Assert.AreEqual(expectedDefaultValue, _processImageData.processRef);
        }

        [Test]
        public void when_a_new_ProcessImageData_domain_object_is_created_its_imageId_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;

            //assert
            Assert.AreEqual(expectedDefaultValue, _processImageData.imageId);
        }
        [Test]
        public void when_a_new_ProcessImageData_domain_object_is_created_its_key_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;

            //assert
            Assert.AreEqual(expectedDefaultValue, _processImageData.key);
        }
        [Test]
        public void when_a_new_ProcessImageData_domain_object_is_created_its_imageData_child_object_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;

            //assert
            Assert.AreEqual(expectedDefaultValue, _processImageData.imageData);
        }

        #endregion

        #region Base64DecodedData

        [Test]
        public void when_a_new_Base64DecodedData_domain_sub_object_is_created_its_imageBytes_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;

            //assert
            Assert.AreEqual(expectedDefaultValue, _base64DecodedData.imagebase64String);
        }

        [Test]
        public void when_a_new_Base64DecodedData_domain_sub_object_is_created_its_imageType_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;

            //assert
            Assert.AreEqual(expectedDefaultValue, _base64DecodedData.imageType);
        }

        [Test]
        public void when_a_new_Base64DecodedData_domain_sub_object_is_created_its_imageExtension_property_value_defaults_to_null()
        {
            //arrange
            string expectedDefaultValue = null;

            //assert
            Assert.AreEqual(expectedDefaultValue, _base64DecodedData.imageExtension);
        }

        #endregion
    }
}

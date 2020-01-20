using Bogus;
using mat_process_api.V1.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Helper
{
    [TestFixture]
    public class ImagePersistingHelperTests
    {
        Faker faker;
        ImagePersistingHelper helper;
        [SetUp]
        public void set_up()
        {
            faker = new Faker();
        }
        [TestCase("2021/01/abc-123-bbb")]
        [TestCase("2020/12/abc-123-ggg")]
        public void test_that_generated_key_is_correct(string imageId)
        {
            //arrange
            var processRef = faker.Random.Guid().ToString();
            var fileExtension = faker.Random.Word();
            var processType = faker.Random.Word();
            var expectedKey = $"{processType}/{processRef}/{imageId}.{fileExtension}";
            //act
            var result = ImagePersistingHelper.generateImageKey(processType,imageId, processRef, fileExtension);
            Assert.AreEqual(expectedKey, result);
        }
    }
}

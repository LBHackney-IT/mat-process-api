using System.Linq;
using Bogus;
using NUnit.Framework;
using mat_process_api.V1.Domain;
using UnitTests.V1.Helper;
using mat_process_api.V1.Gateways;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace UnitTests.V1.Gateways
{
    [TestFixture]
    public class MatProcessesGatewayTests : DbTest { 

         private readonly Faker _faker = new Faker();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void dummy_example_test_tmongo()
        {
            //leave test temporarily for reference
            JObject test = new JObject();
            test.Add("_id",_faker.Random.Guid());
            test.Add("processType", _faker.Random.Word());

            //parse to bson
            var update = BsonDocument.Parse(test.ToString());
            //insert into db
            collection.InsertOne(update);

            //ensure we have inserted document
            Assert.AreEqual(collection.CountDocuments(Builders<BsonDocument>.Filter.Empty),1);
        }
    }
}

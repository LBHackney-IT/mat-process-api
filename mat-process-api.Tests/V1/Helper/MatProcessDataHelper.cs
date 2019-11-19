using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.V1.Domain;
using Newtonsoft.Json.Linq;

namespace mat_process_api.Tests.V1.Helper
{
    public class MatProcessDataHelper
    {
        public static MatProcessData CreateProcessDataObject()
        {
            Faker faker = new Faker();

            return new MatProcessData
            {
                DateCreated = faker.Date.Recent(),
                DateLastModified = faker.Date.Recent(),
                ProcessData = {},
                ProcessDataSchemaVersion = faker.Random.Int(0,10),
                DateCompleted = faker.Date.Recent(),
                Id = faker.Random.Word(),
                PostProcessData = {},
                PreProcessData = {},
                ProcessStage = faker.Random.Word(),
                ProcessType = faker.Random.Word()
            };
        }
    }
}

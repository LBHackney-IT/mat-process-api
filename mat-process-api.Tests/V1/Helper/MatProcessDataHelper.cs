using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Boundary;
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
                Id = faker.Random.Guid().ToString(),
                ProcessType = new ProcessType()
                {
                    value = faker.Random.Int(),
                    name = faker.Random.Word()
                },
                DateCreated = faker.Date.Recent(),
                DateLastModified = faker.Date.Recent(),
                DateCompleted = faker.Date.Recent(),
                ProcessDataAvailable = false,
                ProcessDataSchemaVersion = faker.Random.Int(0,10),
                ProcessStage = faker.Random.Word(),
                LinkedProcessId = null,
                PreProcessData = {},
                ProcessData = {},
                PostProcessData = {},
            };
        }

        public static PostInitialProcessDocumentRequest CreatePostInitialProcessDocumentRequestObject()
        {
            Faker faker = new Faker();

            string processRef = faker.Random.Guid().ToString();
            ProcessType processType = new ProcessType()
            {
                value = faker.Random.Int(),
                name = faker.Random.Word()
            };
            int processDataSchemaVersion = faker.Random.Int();

            return new PostInitialProcessDocumentRequest() { processRef = processRef, processType = processType, processDataSchemaVersion = processDataSchemaVersion };
        }
    }
}

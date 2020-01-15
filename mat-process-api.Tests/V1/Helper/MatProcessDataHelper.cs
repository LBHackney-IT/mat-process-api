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
        private static Faker faker = new Faker();

        public static MatProcessData CreateProcessDataObject()
        {
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
                ProcessDataSchemaVersion = faker.Random.Int(1,10),
                ProcessStage = "Not completed",
                LinkedProcessId = null,
                PreProcessData = {},
                ProcessData = {},
                PostProcessData = {},
            };
        }

        public static PostInitialProcessDocumentRequest CreatePostInitialProcessDocumentRequestObject()
        {
            string processRef = faker.Random.Guid().ToString();
            ProcessType processType = new ProcessType()
            {
                value = faker.Random.Int(),
                name = faker.Random.Word()
            };
            int processDataSchemaVersion = faker.Random.Int();

            return new PostInitialProcessDocumentRequest() { processRef = processRef, processType = processType, processDataSchemaVersion = processDataSchemaVersion };
        }

        public static PostProcessImageRequest CreatePostProcessImageRequestObject()
        {
            return new PostProcessImageRequest()
            {
                processRef = faker.Random.Guid().ToString(),
                imageId = faker.Random.Guid().ToString(),
                base64Image = faker.Random.Hash()
            };
        }
    }
}

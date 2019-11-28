using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Boundary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace mat_process_api.V1.Factories
{
    public class ProcessDataFactory
    {
        public static MatProcessData CreateProcessDataObject(BsonDocument bsonResult)
        {
            if (bsonResult != null)
            {
                return BsonSerializer.Deserialize<MatProcessData>(bsonResult);
            }
            else
            {
                //return empty object, rather than null, as front end will be expecting emtpy object?
                return new MatProcessData();
            }
        }

        public static MatProcessData CreateProcessDataObject(PostInitialProcessDocumentRequest requestObject) //Maps PostInitialDocumentRequest object to ProcessData domain object
        {
            DateTime dateOfCreation = DateTime.Now; // need this here because DateCreated and DateLastModified have to be equal

            return new MatProcessData()
            {
                Id = requestObject.processRef,
                ProcessType = requestObject.processType,
                DateCreated = dateOfCreation,
                DateLastModified = dateOfCreation,
                DateCompleted = DateTime.MinValue,
                ProcessDataSchemaVersion = requestObject.processDataSchemaVersion,
                ProcessStage = "0",
                PreProcessData = new { },
                ProcessData = new { },
                PostProcessData = new { }
            };
        }
    }
}

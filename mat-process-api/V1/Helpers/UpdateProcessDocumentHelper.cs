using mat_process_api.V1.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Helpers
{
    public class UpdateProcessDocumentHelper
{
    public static UpdateDefinition<BsonDocument> PrepareFieldsToBeUpdated(MatUpdateProcessData matProcessData)
    {
        var listOfUpdates = new List<UpdateDefinition<BsonDocument>>();
        //mandatory field
        var dateLastModified = Builders<BsonDocument>.Update.Set("dateLastModified", matProcessData.DateLastModified);
        listOfUpdates.Add(dateLastModified);
        //update has occurred
        var processDataAvailable = Builders<BsonDocument>.Update.Set("processDataAvailable", true);
        listOfUpdates.Add(processDataAvailable);

        if (matProcessData.DateCompleted != DateTime.MinValue)
        {
            var dateCompleted = Builders<BsonDocument>.Update.Set("dateCompleted", matProcessData.DateCompleted);
            listOfUpdates.Add(dateCompleted);
        }
        if (matProcessData.PostProcessData != null)
        {
            var postProcessData = Builders<BsonDocument>.Update.Set("postProcessData",
                BsonDocument.Parse(JsonConvert.SerializeObject(matProcessData.PostProcessData)));
            listOfUpdates.Add(postProcessData);
        }
        if (matProcessData.PreProcessData != null)
        {
            var preProcessData = Builders<BsonDocument>.Update.Set("preProcessData",
                BsonDocument.Parse(JsonConvert.SerializeObject(matProcessData.PreProcessData)));
            listOfUpdates.Add(preProcessData);
        }
        if (matProcessData.ProcessData != null)
        {
            var processData = Builders<BsonDocument>.Update.Set("processData",
                BsonDocument.Parse(JsonConvert.SerializeObject(matProcessData.ProcessData)));
            listOfUpdates.Add(processData);
        }
        if (matProcessData.ProcessDataSchemaVersion != 0)
        {
            var processDataSchemaVersion = Builders<BsonDocument>.Update.Set("dataSchemaVersion", matProcessData.ProcessDataSchemaVersion);
            listOfUpdates.Add(processDataSchemaVersion);
        }
        if (!string.IsNullOrWhiteSpace(matProcessData.ProcessStage))
        {
            var processStage = Builders<BsonDocument>.Update.Set("processStage", matProcessData.ProcessStage);
            listOfUpdates.Add(processStage);
        }

        return Builders<BsonDocument>.Update.Combine(listOfUpdates);
    }
}
}

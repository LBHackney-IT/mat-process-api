using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mat_process_api.V1.Domain
{
    [BsonIgnoreExtraElements]
    public class MatProcessData
    {
        [JsonProperty("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [JsonProperty("processType")]
        [BsonElement("processType")]
        public string ProcessType { get; set; }
        [JsonProperty("dateCreated")]
        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; }
        [JsonProperty("dateLastModified")]
        [BsonElement("dateLastModified")]
        public DateTime DateLastModified { get; set; }
        [JsonProperty("dateCompleted")]
        [BsonElement("dateCompleted")]
        public DateTime DateCompleted { get; set; }
        [JsonProperty("dataSchemaVersion")]
        [BsonElement("dataSchemaVersion")]
        public int ProcessDataSchemaVersion { get; set; }
        [JsonProperty("processStage")]
        [BsonElement("processStage")]
        public string ProcessStage { get; set; }
        [JsonProperty("preProcessData")]
        [BsonElement("preProcessData")]
        public object PreProcessData { get; set; }
        [JsonProperty("processData")]
        [BsonElement("processData")]
        public object ProcessData { get; set; }
        [JsonProperty("postProcessData")]
        [BsonElement("postProcessData")]
        public object PostProcessData { get; set; }
    }
}

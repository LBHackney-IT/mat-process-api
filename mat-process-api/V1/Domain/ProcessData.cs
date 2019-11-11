using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mat_process_api.V1.Domain
{
    public class MatProcessData
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("processType")]
        public string ProcessType { get; set; }
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }
        [JsonProperty("dateLastModified")]
        public DateTime DateLastModified { get; set; }
        [JsonProperty("dateCompleted")]
        public DateTime DateCompleted { get; set; }
        [JsonProperty("dataSchemaVersion")]
        public int DataSchemaVersion { get; set; }
        [JsonProperty("processStage")]
        public string ProcessStage { get; set; }
        [JsonProperty("preProcessData")]
        public object PreProcessData { get; set; }
        [JsonProperty("processData")]
        public object ProcessData { get; set; }
        [JsonProperty("postProcessData")]
        public object PostProcessData { get; set; }
    }
}

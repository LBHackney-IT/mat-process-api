using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;
using MongoDB.Driver;
using MongoDB.Bson;

namespace mat_process_api.V1.Gateways
{
    public interface IProcessDataGateway
    {
        MatProcessData GetProcessData(string processRef);
        MatProcessData UpdateProcessData(UpdateDefinition<BsonDocument> updateDefinition, string id);
        string PostInitialProcessDocument(MatProcessData processDoc);
    }
}

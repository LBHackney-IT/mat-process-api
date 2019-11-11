using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mat_process_api.V1.Gateways
{
    public class ProcessDataGateway : IProcessDataGateway
    {
        public MatProcessData GetProcessData(string processRef)
        {
            //TODO refactor
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", processRef);

           throw new NotImplementedException();
        }
    }
}

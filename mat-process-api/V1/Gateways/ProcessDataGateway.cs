using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mat_process_api.V1.Gateways
{
    public class ProcessDataGateway : IProcessDataGateway
    {
        private IMatDbContext matDbContext;

        public ProcessDataGateway(IMatDbContext _matDbContext)
        {
            matDbContext = _matDbContext;
        }
        public MatProcessData GetProcessData(string processRef)
        {
            //TODO refactor
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", processRef);

            var result = matDbContext.getCollection().FindAsync(filter).Result.FirstOrDefault();

            return ProcessDataFactory.CreateProcessDataObject(result);
        }
    }
}

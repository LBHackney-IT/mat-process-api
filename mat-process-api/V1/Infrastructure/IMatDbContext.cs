using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mat_process_api.V1.Infrastructure
{
    public interface IMatDbContext
    {
        IMongoCollection<BsonDocument> matProcessCollection { get; set; }

        IMongoCollection<BsonDocument> getCollection();

    }
}

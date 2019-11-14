using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;
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
    }
}

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
using Newtonsoft.Json;

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
            //retrieve data by id
            var filter = Builders<BsonDocument>.Filter.Eq("_id", processRef);
            //we will never expect more than one JSON documents matching an ID so we always choose the first/default result
            var result = matDbContext.getCollection().FindAsync(filter).Result.FirstOrDefault();
            
            return ProcessDataFactory.CreateProcessDataObject(result);
        }

        public MatProcessData UpdateProcessData(UpdateDefinition<BsonDocument> updateDefinition, string id)
        {
            //return updated document
            var options = new FindOneAndUpdateOptions<BsonDocument>();
            options.ReturnDocument = ReturnDocument.After;
            //query by
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            //find and update
            var result = matDbContext.getCollection().FindOneAndUpdate(filter, updateDefinition, options);
            //if empty result, the document to be updated was not found
            if(result == null)
            {
                throw new DocumentNotFound();
            }
            return ProcessDataFactory.CreateProcessDataObject(result);
        }
        public string PostInitialProcessDocument(MatProcessData processDoc)
        {
            try
            {
                BsonDocument bsonObject = BsonDocument.Parse(JsonConvert.SerializeObject(processDoc));
                matDbContext.getCollection().InsertOneAsync(bsonObject).Wait(); 
                return processDoc.Id;
            }
            catch(AggregateException ex) //AggregateException - it can wraps up other exceptions while async action is happening. In this case it wraps up the MongoWriteException, which we need to catch. However due to it being wrapped, we can't really catch it...unless you handle the aggregate exception, and extract the Mongo exception. That's what's happening here.
            {
                ex.Handle((x) =>
                {
                    if (x is MongoWriteException) 
                    {
                        throw new ConflictException(ex.Message, ex.InnerException); //throw our custom exception, with the information we need wrapped up.
                    }
                    throw ex;
                });
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
    }

    public class ConflictException : System.Exception
    {
        public ConflictException(String message, Exception inner) : base(message, inner) { }
    }

    public class DocumentNotFound : Exception {}
}

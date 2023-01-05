using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Models
{
    internal class WatchDogMongoModels
    {
        public class MongoWatchLog
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            public string ResponseBody { get; set; }
            public int ResponseStatus { get; set; }
            public string RequestBody { get; set; }
            public string QueryString { get; set; }
            public string Path { get; set; }
            public string RequestHeaders { get; set; }
            public string ResponseHeaders { get; set; }
            public string Method { get; set; }
            public string Host { get; set; }
            public string IpAddress { get; set; }
            public string TimeSpent { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        public class MongoWatchExceptionLog
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string TypeOf { get; set; }
            public string Source { get; set; }
            public string Path { get; set; }
            public string Method { get; set; }
            public string QueryString { get; set; }
            public string RequestBody { get; set; }
            public DateTime EncounteredAt { get; set; }
        }

        public class MongoWatchLoggerModel
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
            public string CallingFrom { get; set; }
            public string CallingMethod { get; set; }
            public int LineNumber { get; set; }
            public string LogLevel { get; set; }
        }
    }
}

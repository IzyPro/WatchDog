﻿using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WatchDog.src.Data;
using WatchDog.src.Models;
using WatchDog.src.Utilities;

namespace WatchDog.src.Helpers
{
    internal class MongoDBHelper
    {
        public static MongoClient mongoClient = ExternalDbContext.CreateMongoDBConnection();
        static IMongoDatabase database = mongoClient.GetDatabase(WatchDogExternalDbConfig.MongoDbName);
        static IMongoCollection<WatchLog> _watchLogs = database.GetCollection<WatchLog>(Constants.WatchLogTableName);
        static IMongoCollection<WatchExceptionLog> _watchExLogs = database.GetCollection<WatchExceptionLog>(Constants.WatchLogExceptionTableName);
        static IMongoCollection<WatchLoggerModel> _logs = database.GetCollection<WatchLoggerModel>(Constants.LogsTableName);
        static IMongoCollection<Sequence> _counter = database.GetCollection<Sequence>(Constants.WatchDogMongoCounterTableName);


        //WATCH lOGS OPERATION
        public static Page<WatchLog> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber, 
            string tag, string eventId, string ipAddress, DateTime? initialTimeStamp, DateTime? finalTimeStamp)
        {
            searchString = searchString?.ToLower();
            var builder = Builders<WatchLog>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(statusCode))
                filter &= builder.Eq(x => x.ResponseStatus, int.Parse(statusCode));

            if (!string.IsNullOrEmpty(verbString))
                filter &= builder.Eq(x => x.Method, verbString);

            if (!string.IsNullOrEmpty(tag))
                filter &= builder.Where(l => l.Tag.ToLower().Contains(tag.ToLower()));    

            if (!string.IsNullOrEmpty(eventId))
                filter &= builder.Where(l => l.EventId.ToLower().Contains(eventId.ToLower()));

            if (!string.IsNullOrEmpty(ipAddress))
                filter &= builder.Where(l => l.IpAddress.Contains(ipAddress));        

            if (initialTimeStamp != null)
                filter &= builder.Gte(x => x.StartTime, initialTimeStamp);

            if (finalTimeStamp != null)
                filter &= builder.Lte(x => x.StartTime, finalTimeStamp);        

            if (!string.IsNullOrEmpty(searchString))
                filter &= builder.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || (!string.IsNullOrEmpty(l.QueryString) && l.QueryString.ToLower().Contains(searchString))
                    || l.RequestBody.ToLower().Contains(searchString));

            var result = _watchLogs.Find(filter).SortByDescending(x => x.Id).ToPaginatedList(pageNumber);
            return result;
        }

        public static async Task InsertWatchLog(WatchLog log)
        {
            log.Id = GetSequenceId();   
             await _watchLogs.InsertOneAsync(log);
        }

        public static async Task<bool> ClearWatchLog()
        {
            var deleteResult = await _watchLogs.DeleteManyAsync(Builders<WatchLog>.Filter.Empty);
            return deleteResult.IsAcknowledged;
        }

        //Watch Exception Operations
        public static Page<WatchExceptionLog> GetAllWatchExceptionLogs(string searchString, int pageNumber, bool negateTypeOf, 
            string typeOf, string tag, string eventId, string ipAddress, DateTime? initialEncounteredAt, DateTime? finalEncounteredAt)
        {
            searchString = searchString?.ToLower();
            var builder = Builders<WatchExceptionLog>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(typeOf))
            {
                if (negateTypeOf)
                    filter &= builder.Where(x => !x.TypeOf.Contains(typeOf));
                else
                    filter &= builder.Where(x => x.TypeOf.Contains(typeOf));
            }

            if (!string.IsNullOrEmpty(tag))
                filter &= builder.Where(l => l.Tag.ToLower().Contains(tag.ToLower()));

            if (!string.IsNullOrEmpty(eventId))
                filter &= builder.Where(l => l.EventId.ToLower().Contains(eventId.ToLower()));

            if (!string.IsNullOrEmpty(ipAddress))
                filter &= builder.Where(l => l.IpAddress.Contains(ipAddress));    

            if (initialEncounteredAt != null)
                filter &= builder.Gte(x => x.EncounteredAt, initialEncounteredAt);
            
            if (finalEncounteredAt != null)
                filter &= builder.Lte(x => x.EncounteredAt, finalEncounteredAt);

            if (!string.IsNullOrEmpty(searchString))
                filter &= builder.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToLower().Contains(searchString) 
                    || l.RequestBody.ToLower().Contains(searchString));

            var result = _watchExLogs.Find(filter).SortByDescending(x => x.Id).ToPaginatedList(pageNumber);
            return result;
        }

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            log.Id = GetSequenceId();
            await _watchExLogs.InsertOneAsync(log);
        }
        public static async Task<bool> ClearWatchExceptionLog()
        {
            var deleteResult = await _watchExLogs.DeleteManyAsync(Builders<WatchExceptionLog>.Filter.Empty);
            return deleteResult.IsAcknowledged;
        }


        //LOGS OPERATION
        public static async Task InsertLog(WatchLoggerModel log)
        {
            log.Id = GetSequenceId();   
            await _logs.InsertOneAsync(log);
        }
        public static async Task<bool> ClearLogs()
        {
            var deleteResult = await _logs.DeleteManyAsync(Builders<WatchLoggerModel>.Filter.Empty);
            return deleteResult.IsAcknowledged;
        }
        public static Page<WatchLoggerModel> GetAllLogs(string searchString, string logLevelString, int pageNumber, string tag, DateTime? initialEncounteredAt, DateTime? finalEncounteredAt)
        {
            searchString = searchString?.ToLower();
            var builder = Builders<WatchLoggerModel>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(tag))
                filter &= builder.Where(l => l.Tag.ToLower().Contains(tag.ToLower()));

            if (initialEncounteredAt != null)
                filter &= builder.Gte(x => x.Timestamp, initialEncounteredAt);
            
            if (finalEncounteredAt != null)
                filter &= builder.Lte(x => x.Timestamp, finalEncounteredAt);
            
            if (!string.IsNullOrEmpty(searchString))
                filter &= builder.Where(l => l.Message.ToLower().Contains(searchString) || l.CallingMethod.ToLower().Contains(searchString) || l.CallingFrom.ToLower().Contains(searchString) || (!string.IsNullOrEmpty(l.EventId) && l.EventId.ToLower().Contains(searchString)));

            if (!string.IsNullOrEmpty(logLevelString))
            {
               filter &= builder.Eq(l => l.LogLevel, logLevelString);
            }

            var result = _logs.Find(filter).SortByDescending(x => x.Id).ToPaginatedList(pageNumber);
            return result;
        }


        public static int GetSequenceId()
        {
            var filter = Builders<Sequence>.Filter.Eq(a => a._Id, "sequenceId");
            var update = Builders<Sequence>.Update.Inc(a => a.Value, 1);
            var sequence = _counter.FindOneAndUpdate(filter, update);

            return sequence.Value;
        }


        public static async Task<bool> ClearAllLogs()
        {
            var watchLogs = await ClearWatchLog();
            var exLogs = await ClearWatchExceptionLog();
            var logs = await ClearLogs();

            return watchLogs && exLogs && logs;
        }
    }
}

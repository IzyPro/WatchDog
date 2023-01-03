using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDog.src.Data;
using WatchDog.src.Models;
using WatchDog.src.Utilities;
using static WatchDog.src.Models.WatchDogMongoModels;

namespace WatchDog.src.Helpers
{
    internal class MongoDBHelper
    {

        public static MongoClient mongoClient = ExternalDbContext.CreateMongoDBConnection();
        static IMongoDatabase database = mongoClient.GetDatabase(Constants.WatchDogDatabaseName);
        static IMongoCollection<WatchLog> _watchLogs = database.GetCollection<WatchLog>(Constants.WatchLogTableName);
        static IMongoCollection<WatchExceptionLog> _watchExLogs = database.GetCollection<WatchExceptionLog>(Constants.WatchLogExceptionTableName);
        static IMongoCollection<WatchLoggerModel> _logs = database.GetCollection<WatchLoggerModel>(Constants.LogsTableName);

        public static Page<WatchLog> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber)
        {
            var query = _watchLogs.AsQueryable<WatchLog>();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || l.ResponseStatus.ToString().Contains(searchString) || (!string.IsNullOrEmpty(l.QueryString) && l.QueryString.ToLower().Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(verbString))
            {
                query.Where(l => l.Method.ToLower() == verbString.ToLower());
            }

            if (!string.IsNullOrEmpty(statusCode))
            {
                query.Where(l => l.ResponseStatus.ToString() == statusCode);
            }
            return query.OrderByDescending(x => x.StartTime).ToPaginatedList(pageNumber);
        }

        //WATCH lOGS OPERATION
        public static async Task<WatchLog> GetWatchLogById(int id)
        {
            return await _watchLogs.Find(x => x.Id == id).Limit(1).SingleAsync();
        }

        public static async Task InsertWatchLog(WatchLog log)
        {
             await _watchLogs.InsertOneAsync(log);
        }

        public static async Task<bool> UpdateWatchLog(WatchLog log)
        {
            var filter = Builders<WatchLog>.Filter.Eq(s => s.Id, log.Id);
            var updateResult = await _watchLogs.ReplaceOneAsync(filter, log);
            return true;
        }

        public static async Task<bool> DeleteWatchLog(int id)
        {
            var deleteResult = await _watchLogs.DeleteOneAsync(x => x.Id == id);
            return deleteResult.IsAcknowledged;
        }

        public static async Task<bool> ClearWatchLog()
        {
            var deleteResult = await _watchLogs.DeleteManyAsync(Builders<WatchLog>.Filter.Empty);
            return deleteResult.IsAcknowledged;
        }

        //Watch Exception Operations
        public static Page<WatchExceptionLog> GetAllWatchExceptionLogs(string searchString, int pageNumber)
        {
            var query = _watchExLogs.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToLower().Contains(searchString));
            }
            return query.OrderByDescending(x => x.EncounteredAt).ToPaginatedList(pageNumber);
        }

        public static async Task<WatchExceptionLog> GetWatchExceptionLogById(int id)
        {
            return await _watchExLogs.Find(x => x.Id == id).Limit(1).SingleAsync();
        }

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            await _watchExLogs.InsertOneAsync(log);
        }

        public static async Task<bool> UpdateWatchExceptionLog(WatchExceptionLog log)
        {
            var filter = Builders<WatchExceptionLog>.Filter.Eq(s => s.Id, log.Id);
            var updateResult = await _watchExLogs.ReplaceOneAsync(filter, log);
            return true;
        }

        public static async Task<bool> DeleteWatchExceptionLog(int id)
        {
            var deleteResult = await _watchExLogs.DeleteOneAsync(x => x.Id == id);
            return deleteResult.IsAcknowledged;
        }
        public static async Task<bool> ClearWatchExceptionLog()
        {
            var deleteResult = await _watchExLogs.DeleteManyAsync(Builders<WatchExceptionLog>.Filter.Empty);
            return deleteResult.IsAcknowledged;
        }


        //LOGS OPERATION
        public static async Task InsertLog(WatchLoggerModel log)
        {
            await _logs.InsertOneAsync(log);
        }
        public static async Task<bool> ClearLogs()
        {
            var deleteResult = await _logs.DeleteManyAsync(Builders<WatchLoggerModel>.Filter.Empty);
            return deleteResult.IsAcknowledged;
        }
        public static Page<WatchLoggerModel> GetAllLogs(string searchString, string logLevelString, int pageNumber)
        {
            var query = _logs.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Message.ToLower().Contains(searchString) || l.CallingMethod.ToLower().Contains(searchString) || l.CallingFrom.ToLower().Contains(searchString));
            }
            if (!string.IsNullOrEmpty(logLevelString))
            {
                query.Where(l => l.LogLevel.ToLower() == logLevelString.ToLower());
            }
            return query.OrderByDescending(x => x.Timestamp).ToPaginatedList(pageNumber);
        }


    }
}

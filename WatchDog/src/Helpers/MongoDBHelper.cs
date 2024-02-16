using MongoDB.Driver;
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
        public static Page<WatchLog> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber)
        {
            searchString = searchString?.ToLower();
            var builder = Builders<WatchLog>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(statusCode))
                filter &= builder.Eq(x => x.ResponseStatus, int.Parse(statusCode));

            if (!string.IsNullOrEmpty(verbString))
                filter &= builder.Eq(x => x.Method, verbString);

            if (!string.IsNullOrEmpty(searchString))
                filter &= builder.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || (!string.IsNullOrEmpty(l.QueryString) && l.QueryString.ToLower().Contains(searchString)));

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
        public static Page<WatchExceptionLog> GetAllWatchExceptionLogs(string searchString, int pageNumber)
        {
            searchString = searchString?.ToLower();
            var builder = Builders<WatchExceptionLog>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(searchString))
                filter &= builder.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToLower().Contains(searchString));

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
        public static Page<WatchLoggerModel> GetAllLogs(string searchString, string logLevelString, int pageNumber)
        {
            searchString = searchString?.ToLower();
            var builder = Builders<WatchLoggerModel>.Filter;
            var filter = builder.Empty;

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

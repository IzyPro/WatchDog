using System;
using System.Threading.Tasks;
using WatchDog.src.Helpers;
using WatchDog.src.Models;

namespace WatchDog.src.Managers
{
    internal static class DynamicDBManager
    {
        internal enum TargetDbEnum
        {
            SqlDb = 0,
            LiteDb,
            MongoDb
        }
        private static string _connectionString = WatchDogExternalDbConfig.ConnectionString;

        private static bool isExternalDb() => !string.IsNullOrEmpty(_connectionString);

        private static TargetDbEnum GetTargetDbEnum
        {
            get{
                if (string.IsNullOrEmpty(_connectionString))
                {
                    return TargetDbEnum.LiteDb;
                }
                if (WatchDogSqlDriverOption.SqlDriverOption != 0)
                {
                    return TargetDbEnum.SqlDb;
                }
                return TargetDbEnum.MongoDb;
            }
        }

        public static async Task<bool> ClearLogs() =>
            GetTargetDbEnum switch
            {
                TargetDbEnum.SqlDb => await ExternalDbHelper.ClearLogs(),
                TargetDbEnum.LiteDb => LiteDBHelper.ClearAllLogs(),
                TargetDbEnum.MongoDb => await MongoDBHelper.ClearAllLogs(),
                _ => throw new NotImplementedException()
            };

        // WATCHLOG OPERATIONS
        public static async Task<Page<WatchLog<object>>> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber) =>
            GetTargetDbEnum switch
            {
                TargetDbEnum.SqlDb => await ExternalDbHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                TargetDbEnum.LiteDb => LiteDBHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                TargetDbEnum.MongoDb => MongoDBHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                _ => throw new NotImplementedException()
            };

        public static async Task InsertWatchLog(WatchLog log)
        {
            var dbOption = GetTargetDbEnum;
            switch (dbOption)
            {
                case TargetDbEnum.SqlDb: 
                    await ExternalDbHelper.InsertWatchLog(log);
                    break;
                case TargetDbEnum.LiteDb:
                    LiteDBHelper.InsertWatchLog(log);
                    break;
                case TargetDbEnum.MongoDb:
                    await MongoDBHelper.InsertWatchLog(log);
                    break;

            }
        }

        // WATCH EXCEPTION OPERATIONS
        public static async Task<Page<WatchExceptionLog>> GetAllWatchExceptionLogs(string searchString, int pageNumber) =>
            GetTargetDbEnum switch
            {
                TargetDbEnum.SqlDb => await ExternalDbHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                TargetDbEnum.LiteDb => LiteDBHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                TargetDbEnum.MongoDb => MongoDBHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                _ => throw new NotImplementedException()

            };

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            var dbOption = GetTargetDbEnum;
            switch (dbOption)
            {
                case TargetDbEnum.SqlDb:
                    await ExternalDbHelper.InsertWatchExceptionLog(log);
                    break;
                case TargetDbEnum.LiteDb:
                    LiteDBHelper.InsertWatchExceptionLog(log);
                    break;
                case TargetDbEnum.MongoDb:
                    await MongoDBHelper.InsertWatchExceptionLog(log);
                    break;
            }
        }

        // LOG OPERATIONS
        public static async Task<Page<WatchLoggerModel>> GetAllLogs(string searchString, string logLevelString, int pageNumber) =>
            GetTargetDbEnum switch
            {
                TargetDbEnum.SqlDb => await ExternalDbHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                TargetDbEnum.LiteDb => LiteDBHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                TargetDbEnum.MongoDb => MongoDBHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                _ => throw new NotImplementedException()
            };

        public static async Task InsertLog(WatchLoggerModel log)
        { 
            var dbOption = GetTargetDbEnum;
            switch (dbOption) {
                case TargetDbEnum.SqlDb: 
                    await ExternalDbHelper.InsertLog(log);
                    break;
                case TargetDbEnum.LiteDb:
                    LiteDBHelper.InsertLog(log);
                    break;
                case TargetDbEnum.MongoDb:
                    await MongoDBHelper.InsertLog(log);
                    break;
            }
        }
    }
}

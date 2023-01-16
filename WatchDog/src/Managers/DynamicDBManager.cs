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

        //private static bool isExternalDb() => !string.IsNullOrEmpty(_connectionString);

        private static TargetDbEnum GetTargetDbEnum
        {
            get {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    return TargetDbEnum.LiteDb;
                }
                if (WatchDogDatabaseDriverOption.DatabaseDriverOption == Enums.WatchDogDbDriverEnum.Mongo)
                {
                    return TargetDbEnum.MongoDb;
                }
                return TargetDbEnum.SqlDb;
            }
        }

        public static async Task<bool> ClearLogs() =>
            GetTargetDbEnum switch
            {
                TargetDbEnum.SqlDb => await SQLDbHelper.ClearLogs(),
                TargetDbEnum.LiteDb => LiteDBHelper.ClearAllLogs(),
                TargetDbEnum.MongoDb => await MongoDBHelper.ClearAllLogs(),
                _ => throw new NotImplementedException()
            };

        // WATCHLOG OPERATIONS
        public static async Task<Page<WatchLog>> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber) =>
            GetTargetDbEnum switch
            {
                TargetDbEnum.SqlDb => await SQLDbHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                TargetDbEnum.LiteDb => LiteDBHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                TargetDbEnum.MongoDb => MongoDBHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                _ => throw new NotImplementedException()
            };

        public static async Task InsertWatchLog(WatchLog log)
        {
            switch (GetTargetDbEnum)
            {
                case TargetDbEnum.SqlDb: 
                    await SQLDbHelper.InsertWatchLog(log);
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
                TargetDbEnum.SqlDb => await SQLDbHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                TargetDbEnum.LiteDb => LiteDBHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                TargetDbEnum.MongoDb => MongoDBHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                _ => throw new NotImplementedException()
            };

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            switch (GetTargetDbEnum)
            {
                case TargetDbEnum.SqlDb:
                    await SQLDbHelper.InsertWatchExceptionLog(log);
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
                TargetDbEnum.SqlDb => await SQLDbHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                TargetDbEnum.LiteDb => LiteDBHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                TargetDbEnum.MongoDb => MongoDBHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                _ => throw new NotImplementedException()
            };

        public static async Task InsertLog(WatchLoggerModel log)
        {
            switch (GetTargetDbEnum) {
                case TargetDbEnum.SqlDb: 
                    await SQLDbHelper.InsertLog(log);
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

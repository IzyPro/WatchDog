using System.Threading.Tasks;
using WatchDog.src.Helpers;
using WatchDog.src.Models;

namespace WatchDog.src.Managers
{
    internal static class DynamicDBManager
    {
        private static string _connectionString = WatchDogExternalDbConfig.ConnectionString;

        private static bool isExternalDb() => !string.IsNullOrEmpty(_connectionString);

        public static async Task<bool> ClearLogs() =>
            isExternalDb() switch
            {
                true => await ExternalDbHelper.ClearLogs(),
                false => LiteDBHelper.ClearAllLogs()
            };
        // WATCHLOG OPERATIONS
        public static async Task<Page<WatchLog>> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber) =>
            isExternalDb() switch
            {
                true => await ExternalDbHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber),
                false => LiteDBHelper.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber)
            };

        public static async Task InsertWatchLog(WatchLog log)
        {
            if (isExternalDb())
            {
                await ExternalDbHelper.InsertWatchLog(log);
            }
            else
            {
                LiteDBHelper.InsertWatchLog(log);
            }
        }

        // WATCH EXCEPTION OPERATIONS
        public static async Task<Page<WatchExceptionLog>> GetAllWatchExceptionLogs(string searchString, int pageNumber) =>
            isExternalDb() switch
            {
                true => await ExternalDbHelper.GetAllWatchExceptionLogs(searchString, pageNumber),
                false => LiteDBHelper.GetAllWatchExceptionLogs(searchString, pageNumber)
            };

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            if (isExternalDb())
            {
                await ExternalDbHelper.InsertWatchExceptionLog(log);
            }
            else
            {
                LiteDBHelper.InsertWatchExceptionLog(log);
            }
        }

        // LOG OPERATIONS
        public static async Task<Page<WatchLoggerModel>> GetAllLogs(string searchString, string logLevelString, int pageNumber) =>
            isExternalDb() switch
            {
                true => await ExternalDbHelper.GetAllLogs(searchString, logLevelString, pageNumber),
                false => LiteDBHelper.GetAllLogs(searchString, logLevelString, pageNumber)
            };

        public static async Task InsertLog(WatchLoggerModel log)
        {
            if (isExternalDb())
                await ExternalDbHelper.InsertLog(log);
            else
                LiteDBHelper.InsertLog(log);
        }
    }
}

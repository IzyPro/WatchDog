using LiteDB;
using WatchDog.src.Models;

namespace WatchDog.src.Helpers
{
    internal static class LiteDBHelper
    {
        public static LiteDatabase db = new LiteDatabase("watchlogs.db");
        static ILiteCollection<WatchLog> _watchLogs = db.GetCollection<WatchLog>("WatchLogs");
        static ILiteCollection<WatchExceptionLog> _watchExLogs = db.GetCollection<WatchExceptionLog>("WatchExceptionLogs");
        static ILiteCollection<WatchLoggerModel> _logs = db.GetCollection<WatchLoggerModel>("Logs");


        //WATCH lOGS OPERATION
        public static Page<WatchLog> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber)
        {
            var query = _watchLogs.Query();
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
            return query.OrderByDescending(x => x.Id).ToPaginatedList(pageNumber);
        }
        public static int InsertWatchLog(WatchLog log)
        {
            return _watchLogs.Insert(log);
        }

        public static int ClearWatchLog()
        {
            return _watchLogs.DeleteAll();
        }


        //Watch Exception Operations
        public static Page<WatchExceptionLog> GetAllWatchExceptionLogs(string searchString, int pageNumber)
        {
            var query = _watchExLogs.Query();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToLower().Contains(searchString));
            }
            return query.OrderByDescending(x => x.Id).ToPaginatedList(pageNumber);
        }

        public static int InsertWatchExceptionLog(WatchExceptionLog log)
        {
            return _watchExLogs.Insert(log);
        }
        public static int ClearWatchExceptionLog()
        {
            return _watchExLogs.DeleteAll();
        }

        //LOGS OPERATION
        public static int InsertLog(WatchLoggerModel log)
        {
            return _logs.Insert(log);
        }
        public static int ClearLogs()
        {
            return _logs.DeleteAll();
        }
        public static Page<WatchLoggerModel> GetAllLogs(string searchString, string logLevelString, int pageNumber)
        {
            var query = _logs.Query();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Message.ToLower().Contains(searchString) || l.CallingMethod.ToLower().Contains(searchString) || l.CallingFrom.ToLower().Contains(searchString) || (!string.IsNullOrEmpty(l.EventId) && l.EventId.ToLower().Contains(searchString)));
            }
            if (!string.IsNullOrEmpty(logLevelString))
            {
                query.Where(l => l.LogLevel.ToLower() == logLevelString.ToLower());
            }
            return query.OrderByDescending(x => x.Id).ToPaginatedList(pageNumber);
        }

        // CLEAR ALL LOGS
        public static bool ClearAllLogs()
        {
            var watchLogs = ClearWatchLog();
            var exLogs = ClearWatchExceptionLog();
            var logs = ClearLogs();

            db.Rebuild();

            return watchLogs > 1 && exLogs > 1 && logs > 1;
        }
    }
}

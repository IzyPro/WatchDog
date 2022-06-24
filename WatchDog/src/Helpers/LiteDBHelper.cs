using LiteDB;
using System.Collections.Generic;
using WatchDog.src.Models;

namespace WatchDog.src.Helpers
{
    internal static class LiteDBHelper
    {
        public static LiteDatabase db = new LiteDatabase("watchlogs.db");
        static ILiteCollection<WatchLog> _watchLogs = db.GetCollection<WatchLog>("WatchLogs");
        static ILiteCollection<WatchExceptionLog> _watchExLogs = db.GetCollection<WatchExceptionLog>("WatchExceptionLogs");
        static ILiteCollection<WatchLoggerModel> _logs = db.GetCollection<WatchLoggerModel>("Logs");

        public static IEnumerable<WatchLog> GetAllWatchLogs()
        {
            return _watchLogs.FindAll();
        }

        public static bool ClearAllLogs()
        {
            var watchLogs = ClearWatchLog();
            var exLogs = ClearWatchExceptionLog();
            var logs = ClearLogs();


            return watchLogs > 1 && exLogs > 1 && logs > 1;
        }

        //WATCH lOGS OPERATION
        public static WatchLog GetWatchLogById(int id)
        {
            return _watchLogs.FindById(id);
        }

        public static int InsertWatchLog(WatchLog log)
        {
            return _watchLogs.Insert(log);
        }

        public static bool UpdateWatchLog(WatchLog log)
        {
            return _watchLogs.Update(log);
        }

        public static bool DeleteWatchLog(int id)
        {
            return _watchLogs.Delete(id);
        }

        public static int ClearWatchLog()
        {
            return _watchLogs.DeleteAll();
        }


        //Watch Exception Operations
        public static IEnumerable<WatchExceptionLog> GetAllWatchExceptionLogs()
        {
            return _watchExLogs.FindAll();
        }

        public static WatchExceptionLog GetWatchExceptionLogById(int id)
        {
            return _watchExLogs.FindById(id);
        }

        public static int InsertWatchExceptionLog(WatchExceptionLog log)
        {
            return _watchExLogs.Insert(log);
        }

        public static bool UpdateWatchExceptionLog(WatchExceptionLog log)
        {
            return _watchExLogs.Update(log);
        }

        public static bool DeleteWatchExceptionLog(int id)
        {
            return _watchExLogs.Delete(id);
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
        public static IEnumerable<WatchLoggerModel> GetAllLogs()
        {
            return _logs.FindAll();
        }
    }
}

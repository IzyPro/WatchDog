using System;
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
        public static Page<WatchLog> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber, 
            string tag, string eventId, string ipAddress, DateTime? initialTimeStamp, DateTime? finalTimeStamp)
        {
            var query = _watchLogs.Query();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || l.ResponseStatus.ToString().Contains(searchString) || (!string.IsNullOrEmpty(l.QueryString) && l.QueryString.ToLower().Contains(searchString))
                    || l.RequestBody.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(verbString))
            {
                query.Where(l => l.Method.ToLower() == verbString.ToLower());
            }

            if (!string.IsNullOrEmpty(statusCode))
            {
                query.Where(l => l.ResponseStatus.ToString() == statusCode);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query.Where(l => l.Tag.ToLower().Contains(tag.ToLower()));
            }

            if (!string.IsNullOrEmpty(eventId))
            {
                query.Where(l => l.EventId.ToLower().Contains(eventId.ToLower()));
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                query.Where(l => l.IpAddress.Contains(ipAddress));
            }

            if (initialTimeStamp != null)
            {
                query.Where(l => l.StartTime >= initialTimeStamp);
            }

            if (finalTimeStamp != null)
            {
                query.Where(l => l.StartTime <= finalTimeStamp);
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
        public static Page<WatchExceptionLog> GetAllWatchExceptionLogs(string searchString, int pageNumber, bool negateTypeOf, 
            string typeOf, string tag, string eventId, string ipAddress, DateTime? initialEncounteredAt, DateTime? finalEncounteredAt)
        {
            var query = _watchExLogs.Query();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToLower().Contains(searchString)
                    || l.RequestBody.ToLower().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(typeOf))
            {
                if (negateTypeOf)
                    query.Where(x => !x.TypeOf.Contains(typeOf));
                else
                    query.Where(x => x.TypeOf.Contains(typeOf));
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query.Where(l => l.Tag.ToLower().Contains(tag.ToLower()));
            }

            if (!string.IsNullOrEmpty(eventId))
            {
                query.Where(l => l.EventId.ToLower().Contains(eventId.ToLower()));
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                query.Where(l => l.IpAddress.Contains(ipAddress));
            }

            if (initialEncounteredAt != null)
            {
                query.Where(l => l.EncounteredAt >= initialEncounteredAt);
            }

            if (finalEncounteredAt != null)
            {
                query.Where(l => l.EncounteredAt <= finalEncounteredAt);
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
        public static Page<WatchLoggerModel> GetAllLogs(string searchString, string logLevelString, int pageNumber, string tag, DateTime? initialEncounteredAt, DateTime? finalEncounteredAt)
        {
            var query = _logs.Query();
            
            if (!string.IsNullOrEmpty(tag))
            {
                query.Where(l => l.Tag.ToLower().Contains(tag.ToLower()));
            }

            if (initialEncounteredAt != null)
            {
                query.Where(l => l.Timestamp >= initialEncounteredAt);
            }

            if (finalEncounteredAt != null)
            {
                query.Where(l => l.Timestamp <= finalEncounteredAt);
            }
            
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

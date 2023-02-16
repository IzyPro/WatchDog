using System.Reflection;

namespace WatchDog.src.Utilities
{
    internal static class Constants
    {
        static Constants()
        {
            string DbPrefix = Assembly.GetEntryAssembly().GetName().Name;
            WatchDogDatabaseName = DbPrefix + "_WatchDogDB";
        }
        public static readonly string WatchDogDatabaseName;
        public const string WatchLogTableName = "WatchDog_WatchLog";
        public const string WatchLogExceptionTableName = "WatchDog_WatchExceptionLog";
        public const string WatchDogMongoCounterTableName = "WatchDog_Counter";
        public const string LogsTableName = "WatchDog_Logs";
        public const int PageSize = 20;
    }
}

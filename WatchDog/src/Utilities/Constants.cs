using System.Reflection;

namespace WatchDog.src.Utilities
{
    internal static class Constants
    {
        public static readonly string DbPrefix = Assembly.GetEntryAssembly().GetName().Name;
        public static readonly string WatchDogDatabaseName = DbPrefix + "_WatchDogDB";
        public const string WatchLogTableName = "WatchDog_WatchLog";
        public const string WatchLogExceptionTableName = "WatchDog_WatchExceptionLog";
        public const string WatchDogMongoCounterTableName = "WatchDog_Counter";
        public const string LogsTableName = "WatchDog_Logs";
        public const int PageSize = 20;
    }
}

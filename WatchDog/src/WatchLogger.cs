using System;
using System.IO;
using System.Runtime.CompilerServices;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog
{
    public class WatchLogger
    {
        public static async void Log(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, string level = "Info")
        {
            var log = new WatchLoggerModel
            {
                Message = message,
                Timestamp = DateTime.Now,
                CallingFrom = Path.GetFileName(filePath),
                CallingMethod = callerName,
                LineNumber = lineNumber,
                LogLevel = level
            }; 

            //Insert
            await DynamicDBManager.InsertLog(log);
            await ServiceProviderFactory.BroadcastHelper.BroadcastLog(log);
        }
        public static void LogError(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, callerName, filePath, lineNumber, "Error");
        }
        public static void LogWarning(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, callerName, filePath, lineNumber, "Warning");
        }
    }
}

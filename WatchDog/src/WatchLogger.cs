using System;
using System.IO;
using System.Runtime.CompilerServices;
using WatchDog.src.Helpers;
using WatchDog.src.Interfaces;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src
{
    public class WatchLogger
    {
        private static readonly IBroadcastHelper _broadcastHelper = new BroadcastHelper(null);
        
        public static async void Log(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            var log = new WatchLoggerModel
            {
                Message = message,
                Timestamp = DateTime.Now,
                CallingFrom = Path.GetFileName(filePath),
                CallingMethod = callerName,
                LineNumber = lineNumber,
            };
            //await DynamicDBManager.InsertLog(log);
            await _broadcastHelper.BroadcastLog(log);
        }
    }
}

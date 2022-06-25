using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using WatchDog.src.Helpers;
using WatchDog.src.Hubs;
using WatchDog.src.Interfaces;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src
{
    public class WatchLogger
    {

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

            //Insert
            await DynamicDBManager.InsertLog(log);
            var service = (IBroadcastHelper)ServiceProviderFactory.ServiceProvider.GetService(typeof(IBroadcastHelper));
            await service.BroadcastLog(log);

        }
    }
}

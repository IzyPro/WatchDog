using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WatchDog.src.Interfaces;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src
{
    internal class WatchDogExceptionLogger
    {
        private readonly RequestDelegate _next;
        private readonly IBroadcastHelper _broadcastHelper;
        public WatchDogExceptionLogger(RequestDelegate next, IBroadcastHelper broadcastHelper)
        {
            _next = next;
            _broadcastHelper = broadcastHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogException(ex, WatchDog.RequestLog);
                throw;
            }
        }
        public async Task LogException(Exception ex, RequestModel requestModel)
        {
            Debug.WriteLine("The following exception is logged: " + ex.Message);
            var watchExceptionLog = new WatchExceptionLog();
            watchExceptionLog.EncounteredAt = DateTime.Now;
            watchExceptionLog.Message = ex.Message;
            watchExceptionLog.StackTrace = ex.StackTrace;
            watchExceptionLog.Source = ex.Source;
            watchExceptionLog.TypeOf = ex.GetType().ToString();
            watchExceptionLog.Path = requestModel?.Path;
            watchExceptionLog.Method = requestModel?.Method;
            watchExceptionLog.QueryString = requestModel?.QueryString;
            watchExceptionLog.RequestBody = requestModel?.RequestBody;

            //Insert
            await DynamicDBManager.InsertWatchExceptionLog(watchExceptionLog);
            await _broadcastHelper.BroadcastExLog(watchExceptionLog);
        }
    }
}

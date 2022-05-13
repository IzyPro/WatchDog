using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
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
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly IBroadcastHelper _broadcastHelper;
        public WatchDogExceptionLogger(RequestDelegate next, ILoggerFactory loggerFactory, IBroadcastHelper broadcastHelper)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<WatchDogExceptionLogger>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
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
                var requestLog = WatchDog.RequestLog;
                await LogException(ex, requestLog);
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

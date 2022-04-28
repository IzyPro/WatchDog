using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDog.src.Helpers;
using WatchDog.src.Interfaces;
using WatchDog.src.Models;

namespace WatchDog.src
{
    public class WatchDogExceptionLogger
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
                var requestLog = await LogRequest(context);
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
            watchExceptionLog.Path = requestModel.Path;
            watchExceptionLog.Method = requestModel.Method;
            watchExceptionLog.RequestBody = requestModel.RequestBody;

            //Insert
            LiteDBHelper.InsertWatchExceptionLog(watchExceptionLog);
            await _broadcastHelper.BroadcastExLog(watchExceptionLog);
        }


        private async Task<RequestModel> LogRequest(HttpContext context)
        {
            var startTime = DateTime.Now;
            List<string> requestHeaders = new List<string>();

            var requestBodyDto = new RequestModel()
            {
                RequestBody = string.Empty,
                Host = context.Request.Host.ToString(),
                Path = context.Request.Path.ToString(),
                Method = context.Request.Method.ToString()
            };


            if (context.Request.ContentLength > 1)
            {
                context.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);
                requestBodyDto.RequestBody = GeneralHelper.ReadStreamInChunks(requestStream);

                context.Request.Body.Position = 0;
            }

            return requestBodyDto;
        }
    }
}

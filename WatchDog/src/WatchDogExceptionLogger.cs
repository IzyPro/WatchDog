using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WatchDog.src.Interfaces;
using WatchDog.src.Managers;
using WatchDog.src.Models;
using WatchDog.src.Helpers;
using System.Linq;
using Microsoft.IO;

namespace WatchDog.src
{
    internal class WatchDogExceptionLogger
    {
        private readonly RequestDelegate _next;
        private readonly IBroadcastHelper _broadcastHelper;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public static RequestModel RequestLog;
        public WatchDogExceptionLogger(RequestDelegate next, IBroadcastHelper broadcastHelper)
        {
            _next = next;
            _broadcastHelper = broadcastHelper;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Exceptions can be generated without having registered a RequestLog
                RequestModel requestLog = WatchDog.RequestLog;
                requestLog ??= await LogRequest(context);
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
            watchExceptionLog.Host = requestModel?.Host;
            watchExceptionLog.IpAddress = requestModel?.IpAddress;
            var timeSpent= watchExceptionLog.EncounteredAt.Subtract(requestModel.StartTime); // This information is important for timeout type exceptions.
            watchExceptionLog.TimeSpent = string.Format("{0:D1} hrs {1:D1} mins {2:D1} secs {3:D1} ms", timeSpent.Hours, timeSpent.Minutes, timeSpent.Seconds, timeSpent.Milliseconds);            
            watchExceptionLog.Tag = requestModel?.Tag;
            watchExceptionLog.EventId = requestModel?.EventId;

            //Insert
            await DynamicDBManager.InsertWatchExceptionLog(watchExceptionLog);
            await _broadcastHelper.BroadcastExLog(watchExceptionLog);
        }    

        // This method is duplicated so as not to break encapsulation
        private async Task<RequestModel> LogRequest(HttpContext context)
        {
            var startTime = DateTime.Now;
            var eventId = string.Empty;

            // Allows you to correlate logging events across different services that are part of the same transaction.
            if(!string.IsNullOrEmpty(WatchDogConfigModel.HeaderNameEventId))
            {
                if (context.Request.Headers.ContainsKey(WatchDogConfigModel.HeaderNameEventId))
                {
                    eventId = context.Request.Headers[WatchDogConfigModel.HeaderNameEventId];
                }   
                else
                {
                    eventId = Guid.NewGuid().ToString();
                    context.Request.Headers.Add(WatchDogConfigModel.HeaderNameEventId, eventId);
                }
            }               

            var requestBodyDto = new RequestModel()
            {
                RequestBody = string.Empty,
                Host = context.Request.Host.ToString(),
                IpAddress = context.Connection.RemoteIpAddress.ToString(),
                Path = context.Request.Path.ToString(),
                Method = context.Request.Method.ToString(),
                QueryString = context.Request.QueryString.ToString(),
                StartTime = startTime,
                Headers = context.Request.Headers.Select(x => x.ToString()).Aggregate((a, b) => a + ": " + b),
                Tag = WatchDogConfigModel.Tag,
                EventId = eventId
            };      

            if (context.Request.ContentLength > 1)
            {
                context.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);
                requestBodyDto.RequestBody = GeneralHelper.ReadStreamInChunks(requestStream);
                context.Request.Body.Position = 0;
            }
            RequestLog = requestBodyDto;
            return requestBodyDto;
        } 
    }
}

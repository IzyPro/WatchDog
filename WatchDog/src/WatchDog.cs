﻿using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WatchDog.src.Helpers;
using WatchDog.src.Interfaces;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src
{
    internal class WatchDog
    {
        public static RequestModel RequestLog;
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly IBroadcastHelper _broadcastHelper;
        private readonly WatchDogOptionsModel _options;

        public WatchDog(WatchDogOptionsModel options, RequestDelegate next, IBroadcastHelper broadcastHelper)
        {
            _next = next;
            _options = options;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _broadcastHelper = broadcastHelper;

            WatchDogConfigModel.UserName = _options.WatchPageUsername;
            WatchDogConfigModel.Password = _options.WatchPagePassword;
            WatchDogConfigModel.Blacklist = String.IsNullOrEmpty(_options.Blacklist) ? new string[] { } : _options.Blacklist.Replace(" ", string.Empty).Split(',');
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var watchLog = new WatchLog();

            if (!context.Request.Path.ToString().Contains("WTCHDwatchpage") && !context.Request.Path.ToString().Contains("watchdog") && !context.Request.Path.ToString().Contains("WTCHDGstatics") && !context.Request.Path.ToString().Contains("favicon") && !context.Request.Path.ToString().Contains("wtchdlogger") && !WatchDogConfigModel.Blacklist.Contains(context.Request.Path.ToString().Remove(0, 1), StringComparer.OrdinalIgnoreCase))
            {
                //Request handling comes here
                var requestLog = await LogRequest(context);
                var responseLog = await LogResponse(context);

                var timeSpent = responseLog.FinishTime.Subtract(requestLog.StartTime);
                //Build General WatchLog, Join from requestLog and responseLog

                watchLog = new WatchLog
                {
                    IpAddress = context.Connection.RemoteIpAddress.ToString(),
                    ResponseStatus = responseLog.ResponseStatus,
                    QueryString = requestLog.QueryString,
                    Method = requestLog.Method,
                    Path = requestLog.Path,
                    Host = requestLog.Host,
                    RequestBody = requestLog.RequestBody,
                    ResponseBody = responseLog.ResponseBody,
                    TimeSpent = string.Format("{0:D1} hrs {1:D1} mins {2:D1} secs {3:D1} ms", timeSpent.Hours, timeSpent.Minutes, timeSpent.Seconds, timeSpent.Milliseconds),
                    RequestHeaders = requestLog.Headers,
                    ResponseHeaders = responseLog.Headers,
                    StartTime = requestLog.StartTime,
                    EndTime = responseLog.FinishTime
                };

                await DynamicDBManager.InsertWatchLog(watchLog);
                await _broadcastHelper.BroadcastWatchLog(watchLog);
            }
            else
            {
                await _next.Invoke(context);
            }
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
                Method = context.Request.Method.ToString(),
                QueryString = context.Request.QueryString.ToString(),
                StartTime = startTime,
                Headers = context.Request.Headers.Select(x => x.ToString()).Aggregate((a, b) => a + ": " + b),
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

        private async Task<ResponseModel> LogResponse(HttpContext context)
        {
            var responseBody = string.Empty;
            using (var originalBodyStream = context.Response.Body)
            {
                try
                {
                    using (var originalResponseBody = _recyclableMemoryStreamManager.GetStream())
                    {
                        context.Response.Body = originalResponseBody;
                        await _next(context);
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var responseBodyDto = new ResponseModel
                        {
                            ResponseBody = responseBody,
                            ResponseStatus = context.Response.StatusCode,
                            FinishTime = DateTime.Now,
                            Headers = context.Response.StatusCode == 200 || context.Response.StatusCode == 201
                                ? string.Empty
                                : context.Response.Headers.Select(x => x.ToString()).Aggregate((a, b) => a + ": " + b),
                        };
                        await originalResponseBody.CopyToAsync(originalBodyStream);
                        return responseBodyDto;
                    }
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WatchDog.src.Helpers;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog
{
    internal class WatchDogLoggerProvider : ILoggerProvider
    {
        private readonly bool _shouldLog;
        private readonly bool _shouldLogCallerInfo;
        public WatchDogLoggerProvider(bool shouldLog, bool shouldLogCallerInfo)
        {
            _shouldLog = shouldLog;
            _shouldLogCallerInfo = shouldLogCallerInfo;
        }
        public ILogger CreateLogger(string CategoryName) => new WatchDogLogger(_shouldLog, _shouldLogCallerInfo, this);

        public void Dispose() { }
    }


    internal class WatchDogLogger : ILogger
    {
        private readonly bool _shouldLog;
        private readonly bool _shouldLogCallerInfo;
        private readonly WatchDogLoggerProvider _loggerProvider;
        public WatchDogLogger(bool shouldLog, bool shouldLogCallerInfo, [NotNull]WatchDogLoggerProvider loggerProvider)
        {
            _shouldLog = shouldLog;
            _shouldLogCallerInfo = shouldLogCallerInfo;
            _loggerProvider = loggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && _shouldLog;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var message = formatter(state, exception) ?? string.Empty;
                var (callerName, filePath, lineNumber) = _shouldLogCallerInfo ? new System.Diagnostics.StackTrace(1, fNeedFileInfo: true).GetFrames().GetCaller() : (string.Empty, string.Empty, 0);
                var eventID = eventId.Name;

                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        WatchLogger.Log(message, eventID, filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                        break;
                    case LogLevel.Warning:
                        WatchLogger.LogWarning(message, eventID, filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        WatchLogger.LogError(message, eventID, filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                        break;
                    default:
                        WatchLogger.Log(message, eventID, filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                        break;
                }
            }
        }
    }


    public class WatchLogger
    {
        public static async void Log(string message, [Optional] string eventId, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, string level = "Info")
        {
            try
            {
                var log = new WatchLoggerModel
                {
                    Message = message,
                    EventId = eventId,
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void LogError(string message, [Optional] string eventId, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, eventId, callerName, filePath, lineNumber, "Error");
        }
        public static void LogWarning(string message, [Optional] string eventId, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, eventId, callerName, filePath, lineNumber, "Warning");
        }
    }
}

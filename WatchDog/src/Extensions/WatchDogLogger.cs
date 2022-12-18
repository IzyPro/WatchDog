using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace WatchDog.Extensions
{
    public sealed class WatchDogLogger : ILogger
    {
        private readonly string _name;
        private readonly Func<WatchDogLoggerConfiguration> _getCurrentConfig;

        public WatchDogLogger(string name, Func<WatchDogLoggerConfiguration> getCurrentConfig) =>
            (_name, _getCurrentConfig) = (name, getCurrentConfig);

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            WatchDogLoggerConfiguration config = _getCurrentConfig();
            string message = $"{_name} - {formatter(state, exception)}";

            StackFrame[] stackFrames = new StackTrace().GetFrames();
            (string filePath, string callerName, int lineNumber) = GetCallerInfo(stackFrames);

            switch (logLevel)
            {
                case LogLevel.None:
                    return;

                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                    WatchLogger.Log(message, level: logLevel.ToString(), filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                    break;

                case LogLevel.Warning:
                    WatchLogger.LogWarning(message, filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                    break;

                case LogLevel.Error:
                case LogLevel.Critical:
                    WatchLogger.LogError(message, filePath: filePath, callerName: callerName, lineNumber: lineNumber);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        private static (string filePath, string callerName, int lineNumber) GetCallerInfo(IEnumerable<StackFrame> frames)
        {
            // Skip frames up to the actual logging call
            foreach (var frame in frames)
            {
                MethodBase method = frame.GetMethod();
                string assemblyName = method?.DeclaringType?.Assembly.GetName().Name;

                switch (assemblyName)
                {
                    case "WatchDog":
                    case "Microsoft.Extensions.Logging":
                    case "Microsoft.Extensions.Logging.Abstractions":
                    case "System.Runtime.CompilerServices":
                        continue;
                }

                return (frame.GetFileName() ?? string.Empty,
                        method?.Name ?? string.Empty,
                        frame.GetFileLineNumber());
            }

            return (string.Empty, string.Empty, 0);
        }
    }
}

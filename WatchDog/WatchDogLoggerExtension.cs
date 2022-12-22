using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog
{
    public static class WatchDogLoggerExtension
    {
        //
        // Summary:
        //     Uses build in ILogger. WARNING: Enabling caller info is performance intensive
        public static ILoggingBuilder AddWatchDogLogger(this ILoggingBuilder builder, bool logCallerInfo = false, bool log = true)
        {
            builder.Services.AddSingleton<ILoggerProvider, WatchDogLoggerProvider>(_ => new WatchDogLoggerProvider(log, logCallerInfo));
            return builder;
        }
    }
}

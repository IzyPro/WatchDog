using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WatchDog
{
    public static class WatchDogLoggerExtension
    {
        //
        // Summary:
        //     Uses built in ILogger. Note: caller info has an overhead of about <= 4ms
        public static ILoggingBuilder AddWatchDogLogger(this ILoggingBuilder builder, bool logCallerInfo = true, bool log = true)
        {
            builder.Services.AddSingleton<ILoggerProvider, WatchDogLoggerProvider>(_ => new WatchDogLoggerProvider(log, logCallerInfo));
            return builder;
        }
    }
}

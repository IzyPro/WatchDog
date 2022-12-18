using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace WatchDog.Extensions
{
    public static class WatchDogLoggerExtensions
    {
        public static ILoggingBuilder AddWatchDogLogger(
            this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WatchDogLoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions
                <WatchDogLoggerConfiguration, WatchDogLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddWatchDogLogger(
            this ILoggingBuilder builder,
            Action<WatchDogLoggerConfiguration> configure)
        {
            builder.AddWatchDogLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}

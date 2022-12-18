using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WatchDog.Extensions
{
    [ProviderAlias("WatchDog")]
    public sealed class WatchDogLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private WatchDogLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, WatchDogLogger> _loggers =
            new ConcurrentDictionary<string, WatchDogLogger>(StringComparer.OrdinalIgnoreCase);

        public WatchDogLoggerProvider(
            IOptionsMonitor<WatchDogLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new WatchDogLogger(name, GetCurrentConfig));

        private WatchDogLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}

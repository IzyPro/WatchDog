using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WatchDog.src.Enums;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src.Services
{
    internal class AutoLogClearerBackgroundService : BackgroundService
    {
        private bool isProcessing;
        private ILogger<AutoLogClearerBackgroundService> logger;
        private readonly IServiceProvider serviceProvider;

        public AutoLogClearerBackgroundService(ILogger<AutoLogClearerBackgroundService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!isProcessing)
                {
                    isProcessing = true;
                }
                else
                {
                    return;
                }

                TimeSpan minute;
                var schedule = AutoClearModel.ClearTimeSchedule;

                switch (schedule)
                {
                    case WatchDogAutoClearScheduleEnum.Hourly:
                        minute = TimeSpan.FromMinutes(60);
                        break;
                    case WatchDogAutoClearScheduleEnum.Every6Hours:
                        minute = TimeSpan.FromHours(6);
                        break;
                    case WatchDogAutoClearScheduleEnum.Every12Hours:
                        minute = TimeSpan.FromHours(12);
                        break;
                    case WatchDogAutoClearScheduleEnum.Daily:
                        minute = TimeSpan.FromDays(1);
                        break;
                    case WatchDogAutoClearScheduleEnum.Weekly:
                        minute = TimeSpan.FromDays(7);
                        break;
                    case WatchDogAutoClearScheduleEnum.Monthly:
                        minute = TimeSpan.FromDays(30);
                        break;
                    case WatchDogAutoClearScheduleEnum.Quarterly:
                        minute = TimeSpan.FromDays(90);
                        break;
                    default:
                        minute = TimeSpan.FromDays(7);
                        break;

                }
                var start = DateTime.UtcNow;
                while (true)
                {
                    var remaining = (minute - (DateTime.UtcNow - start)).TotalMilliseconds;
                    if (remaining <= 0)
                        break;
                    if (remaining > Int16.MaxValue)
                        remaining = Int16.MaxValue;
                    await Task.Delay(TimeSpan.FromMilliseconds(remaining));
                }
                await DoWorkAsync();
                isProcessing = false;
            }
        }

        private async Task DoWorkAsync()
        {
            try
            {
                logger.LogInformation("Log Clearer Background service is starting");
                logger.LogInformation($"Log is clearing...");
                var result = await DynamicDBManager.ClearLogs();
                if(result)
                    logger.LogInformation($"Log Cleared Successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError($"Log Clearer Background service error : {ex.Message}");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Log Clearer Background service is stopping");
            return Task.CompletedTask;
        }
    }
}

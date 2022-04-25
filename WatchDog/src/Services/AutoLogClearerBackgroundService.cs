using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatchDog.src.Enums;
using WatchDog.src.Helpers;
using WatchDog.src.Interfaces;
using WatchDog.src.Models;

namespace WatchDog.src.Services
{
    public class AutoLogClearerBackgroundService : BackgroundService
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


                isProcessing = false;

                TimeSpan minute;
                var schedule = AutoClearModel.ClearTimeSchedule;

                switch (schedule)
                {
                    case WatchDogAutoClearScheduleEnum.Daily:
                        minute = TimeSpan.FromDays(1);
                        break;
                    case WatchDogAutoClearScheduleEnum.Weekly:
                        minute = TimeSpan.FromDays(7);
                        break;
                    case WatchDogAutoClearScheduleEnum.Monthly:
                        minute = TimeSpan.FromDays(30);
                        break;
                    default:
                        minute = TimeSpan.FromDays(7);
                        break;

                }

                await Task.Delay(minute);
                await DoWorkAsync();

            }
        }

        private async Task DoWorkAsync()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var loggerService = scope.ServiceProvider.GetService<ILoggerService>();
                    try
                    {
                        logger.LogInformation("Log Clearer Background service is starting");
                        logger.LogInformation($"Log is clearing...");
                        loggerService.ClearWatchLogs();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Log Clearer Background service error : {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Log Clearer Background service is stopping");
            return Task.CompletedTask;
        }

    }
}

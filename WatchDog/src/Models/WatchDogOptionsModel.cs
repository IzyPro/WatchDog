using System;
using System.Collections.Generic;
using System.Text;
using WatchDog.src.Enums;

namespace WatchDog.src.Models
{
    public class WatchDogOptionsModel
    {
        public string WatchPageUsername { get; set; }
        public string WatchPagePassword { get; set; }
        public bool? WatchDogAutoClearLogs { get; set; }
        public WatchDogAutoClearScheduleEnum WatchDogAutoClearLogsScheduler { get; set; } = WatchDogAutoClearScheduleEnum.Daily;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using WatchDog.src.Enums;

namespace WatchDog.src.Models
{
    public static class WatchDogConfigModel
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static bool IsAutoClear { get; set; }
        public static WatchDogAutoClearScheduleEnum ClearTimeSchedule { get; set; } = WatchDogAutoClearScheduleEnum.Daily;
    }
}

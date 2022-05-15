using WatchDog.src.Enums;

namespace WatchDog.src.Models
{
    public static class WatchDogConfigModel
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string[] Blacklist { get; set; }
    }

    public class WatchDogSettings
    {
        public bool IsAutoClear { get; set; }
        public WatchDogAutoClearScheduleEnum ClearTimeSchedule { get; set; } = WatchDogAutoClearScheduleEnum.Weekly;
        public string SetExternalDbConnString { get; set; } = string.Empty;
        public WatchDogSqlDriverEnum SqlDriverOption { get; set; }
    }

    public static class WatchDogExternalDbConfig
    {
        public static string ConnectionString { get; set; } = string.Empty;
    }

    public static class WatchDogSqlDriverOption
    {
        public static WatchDogSqlDriverEnum SqlDriverOption { get; set; }
    }

    public static class AutoClearModel
    {
        public static bool IsAutoClear { get; set; }
        public static WatchDogAutoClearScheduleEnum ClearTimeSchedule { get; set; } = WatchDogAutoClearScheduleEnum.Weekly;
    }
}

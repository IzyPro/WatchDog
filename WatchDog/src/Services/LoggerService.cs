using System;
using System.Collections.Generic;
using System.Text;
using WatchDog.src.Models;

namespace WatchDog.src.Services
{
    public interface ILoggerService
    {
        void AddToLogs(WatchLog log);

        List<WatchLog> GetAllLogs();
    }
    public class LoggerService
    {
        public void AddToLogs(WatchLog log)
        {
            LoggerStore.Logs.Add(log);
        }

        public List<WatchLog> GetAllLogs()
        {
            return LoggerStore.Logs;
        }
    }

    public class LoggerStore
    {
        public static List<WatchLog> Logs = new List<WatchLog>();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using WatchDog.src.Models;

namespace WatchDog.src.Services
{
    public interface ILoggerService
    {
        void AddToLogs(Request log);

        List<Request> GetAllLogs();
    }
    public class LoggerService
    {
        public void AddToLogs(Request log)
        {
            LoggerStore.Logs.Add(log);
        }

        public List<Request> GetAllLogs()
        {
            return LoggerStore.Logs;
        }
    }

    public class LoggerStore
    {
        public static List<Request> Logs = new List<Request>();
    }
}

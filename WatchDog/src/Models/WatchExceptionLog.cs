using System;

namespace WatchDog.src.Models
{
    public class WatchExceptionLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string TypeOf { get; set; }
        public string Source { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string QueryString { get; set; }
        public string RequestBody { get; set; }
        public DateTime EncounteredAt { get; set; }
        public string Host { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty; 
        public string TimeSpent { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
    }
}

using System;

namespace WatchDog.src.Models
{
    public class WatchLog
    {
        public int Id { get; set; }
        public string ResponseBody { get; set; }
        public int ResponseStatus { get; set; }
        public string RequestBody { get; set; }
        public string QueryString { get; set; }
        public string Path { get; set; }
        public string RequestHeaders { get; set; }
        public string ResponseHeaders { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string IpAddress { get; set; }
        public string TimeSpent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

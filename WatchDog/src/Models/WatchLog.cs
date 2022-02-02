using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Models
{
    public class WatchLog
    {
        public string ResponseBody { get; set; }
        public int ResponseStatus { get; set; }
        public string RequestBody { get; set; }
        public string QueryString { get; set; }
        public string Path { get; set; }
        public string Headers { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public string IpAddress { get; set; }
        public double TimeSpent { get; set; }
    }
}
        
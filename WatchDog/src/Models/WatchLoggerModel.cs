using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Models
{
    public class WatchLoggerModel
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string CallingFrom { get; set; }
        public string CallingMethod { get; set; }
        public int LineNumber { get; set; }
    }
}

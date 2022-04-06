using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Models
{
    public class WatchExceptionLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string TypeOf { get; set; }
        public string Source { get; set; }
        public DateTime EncounteredAt { get; set; }
    }
}

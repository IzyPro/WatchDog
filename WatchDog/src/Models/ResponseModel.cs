using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Models
{
    public class ResponseModel
    {
        public string ResponseBody { get; set; }
        public int ResponseStatus { get; set; }
        public string Headers { get; set; }
        public DateTime FinishTime { get; set; }
    }
}

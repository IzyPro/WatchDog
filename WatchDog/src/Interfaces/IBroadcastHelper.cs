using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WatchDog.src.Models;

namespace WatchDog.src.Interfaces
{
    public interface IBroadcastHelper
    {
        Task BroadcastLog(WatchLog log);
    }
}

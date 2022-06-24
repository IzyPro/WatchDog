using System.Threading.Tasks;
using WatchDog.src.Models;

namespace WatchDog.src.Interfaces
{
    public interface IBroadcastHelper
    {
        Task BroadcastWatchLog(WatchLog log);
        Task BroadcastExLog(WatchExceptionLog log);
        Task BroadcastLog(WatchLoggerModel log);
    }
}

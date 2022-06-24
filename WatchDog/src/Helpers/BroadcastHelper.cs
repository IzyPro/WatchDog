using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WatchDog.src.Hubs;
using WatchDog.src.Interfaces;
using WatchDog.src.Models;

namespace WatchDog.src.Helpers
{
    internal class BroadcastHelper : IBroadcastHelper
    {
        private readonly IHubContext<LoggerHub> _hubContext;
        public BroadcastHelper(IHubContext<LoggerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastWatchLog(WatchLog log)
        {
            var result = new { log = log, type = "rqLog" };
            await _hubContext.Clients.All.SendAsync("getLogs", result);
        }

        public async Task BroadcastLog(WatchLoggerModel log)
        {
            var result = new { log = log, type = "log" };
            await _hubContext.Clients.All.SendAsync("getLogs", result);
        }

        public async Task BroadcastExLog(WatchExceptionLog log)
        {
            var result = new { log = log, type = "exLog" };
            await _hubContext.Clients.All.SendAsync("getLogs", result);
        }
    }
}

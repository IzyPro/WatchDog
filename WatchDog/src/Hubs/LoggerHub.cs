using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatchDog.src.Helpers;
using WatchDog.src.Models;

namespace WatchDog.src.Hubs
{
    public class LoggerHub : Hub
    {
        public async Task GetAllLogs()
        {
            var logs = LiteDBHelper.GetAll();
            if(logs != null)
                logs.OrderBy(x => x.StartTime);
            await Clients.All.SendAsync("GetWatchLogs", logs).ConfigureAwait(false);
        }

        public async void OnChange()
        {
            await base.OnConnectedAsync();

            //{
            //    LoggerHub loggerHub = new LoggerHub();
            //    await loggerHub.GetAllLogs();
            //}
        }
    }
}

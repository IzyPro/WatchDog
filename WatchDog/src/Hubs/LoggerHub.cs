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
        public async Task GetAllLogs(string id)
        {
            var logs = LiteDBHelper.GetAllWatchLogs();
            if (logs != null)
                logs.OrderBy(x => x.StartTime);
            await Clients.All.SendAsync("getAllLogs", logs).ConfigureAwait(false);
            //await Clients.Client(id).SendAsync("getAllLogs", logs);
        }

        //public async Task Send(string message1, string Root)
        //{
        //    await Clients.All.SendAsync("Send", message1);
        //}

        //public async void OnChange()
        //{
        //    await base.OnConnectedAsync();

        //    //{
        //    //    LoggerHub loggerHub = new LoggerHub();
        //    //    await loggerHub.GetAllLogs();
        //    //}
        //}

    }
}

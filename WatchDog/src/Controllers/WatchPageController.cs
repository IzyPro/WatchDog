using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatchDog.src.Helpers;
using WatchDog.src.Models;

namespace WatchDog.src.Controllers
{
    public class WatchPageController : Controller
    {
        public JsonResult Index()
        {
            var logs = LiteDBHelper.GetAllWatchLogs();
            if (logs != null)
                logs.OrderBy(x => x.StartTime);
            return Json(logs);
        }
    }
}

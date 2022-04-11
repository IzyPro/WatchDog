using Microsoft.AspNetCore.Http;
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
        int PAGE_SIZE = 50;
        public JsonResult Index(string searchString = "", int pageNumber = 1)
        {
            searchString = searchString.ToLower();
            var logs = LiteDBHelper.GetAllWatchLogs();
            if (logs != null)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    logs = logs.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || l.QueryString.ToLower().Contains(searchString) || l.ResponseStatus.ToString().Contains(searchString)).OrderByDescending(l => l.StartTime);
                    return Json(PaginatedList<WatchLog>.CreateAsync(logs, pageNumber, PAGE_SIZE));
                }
            }
            logs = logs.OrderByDescending(x => x.StartTime);
            return Json(PaginatedList<WatchLog>.CreateAsync(logs, pageNumber, PAGE_SIZE));
        }

        public JsonResult ClearLogs()
        {
            var cleared = LiteDBHelper.ClearWatchLog();
            return Json(cleared > 0);
        }


        [HttpPost]
        public JsonResult Auth(string username, string password)
        {

            if (username == "username" && password == "password")
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}

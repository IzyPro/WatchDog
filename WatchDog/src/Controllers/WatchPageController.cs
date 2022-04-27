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
        int PAGE_SIZE = 10;
        public JsonResult Index(string searchString = "", string verbString = "", string statusCode = "", int pageNumber = 1)
        {
            var logs = LiteDBHelper.GetAllWatchLogs();
            if (logs != null)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    // todo: Search Query Strings
                    // String.IsNullOrEmpty(l.QueryString) ? false : l.QueryString.ToLower().Contains(searchString) doesn't work for some reason
                    logs = logs.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || l.ResponseStatus.ToString().Contains(searchString));
                }

                if (!string.IsNullOrEmpty(verbString))
                {
                    logs = logs.Where(l => l.Method.ToLower() == verbString.ToLower());
                }

                if (!string.IsNullOrEmpty(statusCode))
                {
                    logs = logs.Where(l => l.ResponseStatus.ToString() == statusCode);
                }
            }
            logs = logs.OrderByDescending(x => x.StartTime);
            var result = PaginatedList<WatchLog>.CreateAsync(logs, pageNumber, PAGE_SIZE);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result });
        }

        public JsonResult Exceptions(string searchString = "", int pageNumber = 1)
        {
            var logs = LiteDBHelper.GetAllWatchExceptionLogs();
            if (logs != null)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    logs = logs.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToString().Contains(searchString));
                }
            }
            logs = logs.OrderByDescending(x => x.EncounteredAt);
            var result = PaginatedList<WatchExceptionLog>.CreateAsync(logs, pageNumber, PAGE_SIZE);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result });
        }

        public JsonResult ClearLogs()
        {
            var cleared = LiteDBHelper.ClearWatchLog();
            return Json(cleared > 0);
        }


        [HttpPost]
        public JsonResult Auth(string username, string password)
        {

            if (username.ToLower() == WatchDogConfigModel.UserName.ToLower() && password == WatchDogConfigModel.Password)
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

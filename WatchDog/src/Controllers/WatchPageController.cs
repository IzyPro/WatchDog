using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src.Controllers
{
    [AllowAnonymous]
    public class WatchPageController : Controller
    {
        public async Task<JsonResult> Index(string searchString = "", string verbString = "", string statusCode = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber);
            //if (logs != null)
            //{
            //    if (!string.IsNullOrEmpty(searchString))
            //    {
            //        searchString = searchString.ToLower();
            //        logs = logs.Where(l => l.Path.ToLower().Contains(searchString) || l.Method.ToLower().Contains(searchString) || l.ResponseStatus.ToString().Contains(searchString) || (!String.IsNullOrEmpty(l.QueryString) && l.QueryString.ToLower().Contains(searchString)));
            //    }

            //    if (!string.IsNullOrEmpty(verbString))
            //    {
            //        logs = logs.Where(l => l.Method.ToLower() == verbString.ToLower());
            //    }

            //    if (!string.IsNullOrEmpty(statusCode))
            //    {
            //        logs = logs.Where(l => l.ResponseStatus.ToString() == statusCode);
            //    }
            //}
            //logs = logs.OrderByDescending(x => x.StartTime);
            //var result = PaginatedList<WatchLog>.CreateAsync(logs, pageNumber, Constants.PageSize);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }

        public async Task<JsonResult> Exceptions(string searchString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchExceptionLogs(searchString, pageNumber);
            //if (logs != null)
            //{
            //    if (!string.IsNullOrEmpty(searchString))
            //    {
            //        searchString = searchString.ToLower();
            //        logs = logs.Where(l => l.Message.ToLower().Contains(searchString) || l.StackTrace.ToLower().Contains(searchString) || l.Source.ToString().Contains(searchString));
            //    }
            //}
            //logs = logs.OrderByDescending(x => x.EncounteredAt);
            //var result = PaginatedList<WatchExceptionLog>.CreateAsync(logs, pageNumber, Constants.PageSize);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }
        public async Task<JsonResult> Logs(string searchString = "", string logLevelString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllLogs(searchString, logLevelString, pageNumber);
            //if (logs != null)
            //{
            //    if (!string.IsNullOrEmpty(searchString))
            //    {
            //        searchString = searchString.ToLower();
            //        logs = logs.Where(l => l.Message.ToLower().Contains(searchString) || l.CallingMethod.ToLower().Contains(searchString) || l.CallingFrom.ToString().Contains(searchString));
            //    }
            //}
            //logs = logs.OrderByDescending(x => x.Timestamp);
            //var result = PaginatedList<WatchLoggerModel>.CreateAsync(logs, pageNumber, Constants.PageSize);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result .Data});
        }

        public async Task<JsonResult> ClearLogs()
        {
            var cleared = await DynamicDBManager.ClearLogs(); 
            return Json(cleared);
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

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
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }

        public async Task<JsonResult> Exceptions(string searchString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchExceptionLogs(searchString, pageNumber);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data });
        }
        public async Task<JsonResult> Logs(string searchString = "", string logLevelString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllLogs(searchString, logLevelString, pageNumber);
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

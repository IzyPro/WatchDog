using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using WatchDog.src.Filters;
using WatchDog.src.Helpers;
using WatchDog.src.Managers;
using WatchDog.src.Models;

namespace WatchDog.src.Controllers
{
    [AllowAnonymous]
    public class WatchPageController : Controller
    {
        private readonly IMemoryCache _cache;
        public WatchPageController(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [CustomAuthenticationFilter]
        public async Task<JsonResult> Index(string searchString = "", string verbString = "", string statusCode = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchLogs(searchString, verbString, statusCode, pageNumber);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data }, GeneralHelper.CamelCaseSerializer);
        }

        [CustomAuthenticationFilter]
        public async Task<JsonResult> Exceptions(string searchString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllWatchExceptionLogs(searchString, pageNumber);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result.Data }, GeneralHelper.CamelCaseSerializer);
        }

        [CustomAuthenticationFilter]
        public async Task<JsonResult> Logs(string searchString = "", string logLevelString = "", int pageNumber = 1)
        {
            var result = await DynamicDBManager.GetAllLogs(searchString, logLevelString, pageNumber);
            return Json(new { PageIndex = result.PageIndex, TotalPages = result.TotalPages, HasNext = result.HasNextPage, HasPrevious = result.HasPreviousPage, logs = result .Data}, GeneralHelper.CamelCaseSerializer);
        }

        [CustomAuthenticationFilter]
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
                _cache.Set("isAuth", "true", GeneralHelper.cacheEntryOptions);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public JsonResult LogOut()
        {
            _cache.Remove("isAuth");
            return Json(true); 
        }

        public JsonResult IsAuth()
        {
            if (!_cache.TryGetValue("isAuth", out string isAuth))
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
    }
}

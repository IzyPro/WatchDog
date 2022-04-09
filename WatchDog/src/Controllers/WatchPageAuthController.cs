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
    public class WatchPageAuthController : Controller
    {


        [HttpPost]
        public JsonResult Index(string username, string password)
        {

            if (username.ToLower() == WatchDogAuthConfigModel.UserName.ToLower() && password == WatchDogAuthConfigModel.Password)
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

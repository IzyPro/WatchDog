using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatchDog.src.Helpers;
using WatchDog.src.Models;

namespace WatchDog.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WatchPageController : Controller
    {
        [HttpGet]
        public IEnumerable<WatchLog> GetAllLogs()
        {
            var logs = LiteDBHelper.GetAll();
            if (logs != null)
                logs.OrderBy(x => x.StartTime);
            return logs;
        }
    }
}

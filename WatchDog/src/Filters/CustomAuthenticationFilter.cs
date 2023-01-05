
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog.src.Filters
{
    internal class CustomAuthenticationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();
            if (!_cache.TryGetValue("isAuth", out string isAuth))
            {
                context.Result = new UnauthorizedResult();
            }
                
        }
    }
}

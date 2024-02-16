using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WatchDog.src.Filters
{
    internal class CustomAuthenticationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            if (!context.HttpContext.Session.TryGetValue("isAuth", out var isAuth))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}

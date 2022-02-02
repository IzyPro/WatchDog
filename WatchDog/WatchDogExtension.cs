using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace WatchDog
{
    public static class WatchDogExtension
    {
        public static IApplicationBuilder UseWatchDog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<src.WatchDog>();
        }
    }
}

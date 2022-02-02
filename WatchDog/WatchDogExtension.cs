using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WatchDog
{
    public static class WatchDogExtension
    {
        public static readonly IFileProvider Provider = new EmbeddedFileProvider(
        typeof(WatchDogExtension).GetTypeInfo().Assembly,
        "WatchDog"
        );


        public static IApplicationBuilder UseWatchDog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<src.WatchDog>();
        }

        public static IApplicationBuilder UseWatchDogPage(this IApplicationBuilder app)
        {
            return app.UseRouter(router => {
                router.MapGet("watchdog", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(WatchDogExtension.GetFile());
                });
            });
        }

        

        public static IFileInfo GetFile()
        {
            return Provider.GetFileInfo("WatchPage.index.html");
        }
    }
}

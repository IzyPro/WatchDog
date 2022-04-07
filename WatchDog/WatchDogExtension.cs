using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WatchDog.src.Controllers;
using WatchDog.src.Helpers;
using WatchDog.src.Hubs;
using WatchDog.src.Interfaces;

namespace WatchDog
{
    public static class WatchDogExtension
    {
        public static readonly IFileProvider Provider = new EmbeddedFileProvider(
        typeof(WatchDogExtension).GetTypeInfo().Assembly,
        "WatchDog"
        );

        public static IServiceCollection AddWatchDogServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddMvcCore(x =>
            {
                x.EnableEndpointRouting = false;
            }).AddApplicationPart(typeof(WatchDogExtension).Assembly);
            services.AddTransient<IBroadcastHelper, BroadcastHelper>();
            return services;
        }
        public static IApplicationBuilder UseWatchDog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<src.WatchDog>();
        }
        public static IApplicationBuilder UseWatchDogExceptionLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<src.WatchDogExceptionLogger>();
        }

        public static IApplicationBuilder UseWatchDogPage(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(WatchDogExtension.GetFolder(), @"src\WatchPage")),

                RequestPath = new PathString("/statics")
            });

            app.UseSignalR(route =>
            {
                route.MapHub<LoggerHub>("/logger");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "watchpage",
                    template: "watchpage",
                    defaults: new { controller = "WatchPage", action = "Index" });
            });

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
            return Provider.GetFileInfo("src.WatchPage.index.html");
        
        }

        public static string GetFolder()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}

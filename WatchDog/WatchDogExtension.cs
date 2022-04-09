using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
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
using WatchDog.src.Models;

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
        public static IApplicationBuilder UseWatchDog(this IApplicationBuilder app, Action<WatchDogAuthModel> configureOptions)
        {
            var options = new WatchDogAuthModel();
            configureOptions(options);


            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(WatchDogExtension.GetFolder(), @$"src{Path.DirectorySeparatorChar}WatchPage")),

                RequestPath = new PathString("/WTCHDGstatics")
            });

            app.UseSignalR(route =>
            {
                route.MapHub<LoggerHub>("/wtchdlogger");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                     name: "WTCHDwatchpage",
                     template: "WTCHDwatchpage",
                     defaults: new { controller = "WatchPage", action = "Index" });

                routes.MapRoute(
                     name: "WTCHDwatchpageauth",
                     template: "WTCHDwatchpageauth",
                     defaults: new { controller = "WatchPageAuth", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            app.UseRouter(router => {

                router.MapGet("watchdog", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(WatchDogExtension.GetFile());

                });

            });

            return app.UseMiddleware<src.WatchDog>(options);
        }
        public static IApplicationBuilder UseWatchDogExceptionLogger(this IApplicationBuilder builder)
        {
           
            return builder.UseMiddleware<src.WatchDogExceptionLogger>();
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WatchDog.src.Helpers;
using WatchDog.src.Hubs;
using WatchDog.src.Interfaces;
using WatchDog.src.Models;
using WatchDog.src.Services;

namespace WatchDog
{
    public static class WatchDogExtension
    {
        public static readonly IFileProvider Provider = new EmbeddedFileProvider(
        typeof(WatchDogExtension).GetTypeInfo().Assembly,
        "WatchDog"
        );

        public static IServiceCollection AddWatchDogServices(this IServiceCollection services, [Optional] Action<AutoClearLogsModel> configureOptions)
        {
            var options = new AutoClearLogsModel();
            if (configureOptions != null)
                configureOptions(options);
            AutoClearModel.IsAutoClear = options.IsAutoClear;
            AutoClearModel.ClearTimeSchedule = options.ClearTimeSchedule;

            services.AddSignalR();
            services.AddMvcCore(x =>
            {
                x.EnableEndpointRouting = false;
            }).AddApplicationPart(typeof(WatchDogExtension).Assembly);
            services.AddTransient<IBroadcastHelper, BroadcastHelper>();
            services.AddTransient<ILoggerService, LoggerService>();
            if (AutoClearModel.IsAutoClear)
                services.AddHostedService<AutoLogClearerBackgroundService>();
            return services;
        }

        public static IApplicationBuilder UseWatchDogExceptionLogger(this IApplicationBuilder builder)
        {

            return builder.UseMiddleware<src.WatchDogExceptionLogger>();
        }

        public static IApplicationBuilder UseWatchDog(this IApplicationBuilder app, Action<WatchDogOptionsModel> configureOptions)
        {
            var options = new WatchDogOptionsModel();
            configureOptions(options);
            if (string.IsNullOrEmpty(options.WatchPageUsername) || string.IsNullOrEmpty(options.WatchPagePassword))
                throw new ArgumentException("Parameters Username and password are required on .UseWatchLog()");

            app.UseMiddleware<src.WatchDog>(options);


            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new EmbeddedFileProvider(
                    typeof(WatchDogExtension).GetTypeInfo().Assembly,
                  "WatchDog.src.WatchPage"),

                RequestPath = new PathString("/WTCHDGstatics")
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LoggerHub>("/wtchdlogger");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "WTCHDwatchpage",
                    template: "WTCHDwatchpage/{action}",
                    defaults: new { controller = "WatchPage", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            app.Build();
            return app.UseRouter(router =>
            {
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchDog;
using WatchDog.src.Enums;

namespace WatchDogCompleteTestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddWatchDogServices();
            //services.AddWatchDogServices(opt => { opt.IsAutoClear = true; opt.SetExternalDbConnString = "Server=(localdb)\\mssqllocaldb;Database=test;Trusted_Connection=True;"; opt.SqlDriverOption = WatchDogSqlDriverEnum.MSSQL; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWatchDogExceptionLogger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseWatchDog(opt => { opt.WatchPageUsername = "admin"; opt.WatchPagePassword = "Qwerty@123"; opt.Blacklist = "Test/testPost, weatherforecast"; });
        }
    }
}

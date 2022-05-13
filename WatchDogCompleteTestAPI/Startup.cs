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
            //services.AddWatchDogServices();
            //services.AddWatchDogServices(opt => { opt.IsAutoClear = true; opt.SetExternalDbConnString = "Server=(localdb)\\mssqllocaldb;Database=test;Trusted_Connection=True;"; opt.SqlDriverOption = WatchDogSqlDriverEnum.MSSQL; });
            //services.AddWatchDogServices(opt => { opt.IsAutoClear = false; opt.SetExternalDbConnString = "Data Source=SQL8003.site4now.net;Initial Catalog=db_a86d7e_wdtest;User Id=db_a86d7e_wdtest_admin;Password=P@ssw0rd"; opt.SqlDriverOption = WatchDogSqlDriverEnum.MSSQL; });
            //services.AddWatchDogServices(opt => { opt.IsAutoClear = false; opt.SetExternalDbConnString = "Server=MYSQL8001.site4now.net;Database=db_a86d7e_wdtest;Uid=a86d7e_wdtest;Pwd=P@ssw0rd"; opt.SqlDriverOption = WatchDogSqlDriverEnum.MySql; });
            services.AddWatchDogServices(opt => { opt.IsAutoClear = false; opt.SetExternalDbConnString = "Server=tyke.db.elephantsql.com;Database=fldwcbfm;User Id=fldwcbfm;Password=qc9cwxXkxkpRfKLoubnaJvm68waPLK2b;"; opt.SqlDriverOption = WatchDogSqlDriverEnum.PostgreSql; });
            
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

            app.UseWatchDog(opt => { opt.WatchPageUsername = "admin"; opt.WatchPagePassword = "Qwerty@123"; opt.Blacklist = "Test/testPost, weatherforecast"; });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}

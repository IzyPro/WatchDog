using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WatchDog;
using WatchDog.src.Enums;

namespace WatchDogTestApi
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
            services.AddWatchDogServices(opt =>
            {
                opt.IsAutoClear = true;
                opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseWatchDogExceptionLogger();
            app.UseWatchDog(opt => {
                opt.WatchPagePassword = "Qwerty@123";
                opt.WatchPageUsername = "Admin";
                opt.Blacklist = "ogedengbe, marerin, otp/2";
            });


            app.UseRouting();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploreCalifornia.Models;
using ExploreCalifornia.ViewComponents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ExploreCalifornia
{
    public class Startup
    {
        private readonly IConfigurationRoot configuration;

        public Startup(IHostingEnvironment env)
        {
            configuration = new ConfigurationBuilder().AddEnvironmentVariables()
              .AddJsonFile(env.ContentRootPath + "/config.json")
              .AddJsonFile(env.ContentRootPath + "/config.Development.json", true)
              .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<FormattingService>();
            #region Depricated - Must Remove Component to become DBContext
            //services.AddTransient<MonthlySpecialsViewComponent>();
            #endregion
            services.AddTransient<SpecialsDataContext>();

            services.AddTransient<FeatureToggles>(x => new FeatureToggles()
            {
                EnableDeveloperExceptions = configuration.GetValue<bool>("FeatureToggles:EnableDeveloperExceptions")
            });

            services.AddDbContext<BlogDataContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("BlogDataContext");
                options.UseSqlServer(connectionString);

            });

            services.AddDbContext<SpecialsDataContext>(options =>
            {
                //Erik - 10/6/2017 Appears they need to be different databases bc they have different dbContext...assume 1 dbContext per Database
                var connectionString = configuration.GetConnectionString("SpecialsDataContext");
                options.UseSqlServer(connectionString);

            });

            services.AddDbContext<IdentityDataContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("IdentityDataContext");
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, FeatureToggles features)
        {
            loggerFactory.AddConsole();

            app.UseExceptionHandler("/error.html");


            #region Depricated - Use Injected FeaturesToggle
            //if (configuration.GetValue<bool>("FeatureToggles:EnableDeveloperExceptions"))
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            #endregion

            if (features.EnableDeveloperExceptions)
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (AppContext, next) =>
            {
                if (AppContext.Request.Path.Value.Contains("invalid"))
                {
                    throw new Exception("Error2!");
                }
                await next();
            });

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute("Default",
                    "{controller=Home}/{action=Index}/{id:int?}");
            });

            app.UseFileServer();

        }
    }
}

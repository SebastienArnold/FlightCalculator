using System.Data.SqlClient;
using FlightCalculator.Core.Interfaces;
using FlightCalculator.DataAccessLayer;
using FlightCalculator.DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlightCalculator.Host
{
    /// <summary>
    /// Startup class of the host application.
    /// Define the request handling pipeline,  any services needed by the app are configured here.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            if (!int.TryParse(Configuration["Services:Webservicex:CacheRetentionMinute"],
                out var webservicexCacheRetention))
            {
                webservicexCacheRetention = 120;
            }

            services.AddMvc();
            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<IDbProvider>(s => new SqlLiteProvider(Configuration["Database:ConnectionString"]));
            services.AddTransient<IFlightCalculator, Business.FlightCalculator>();
            services.AddSingleton<IAircraftRepository, AircraftRepository>();
            services.AddTransient<IAirportRepository>(s => new AirportRepository(
                serviceProvider.GetService<ILoggerFactory>(), serviceProvider.GetService<IMemoryCache>(),
                Configuration["Services:Webservicex:Host"], webservicexCacheRetention,
                Configuration["Services:Webservicex:ProxyUrl"],
                Configuration["Services:Webservicex:ProxyUser"], Configuration["Services:Webservicex:ProxyPassword"]));
            services.AddSingleton<IFlightRepository, FlightRepository>();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Report}");

                routes.MapRoute(
                    name: "aircraft",
                    template: "{controller=Aircraft}/{action=List}/{SelectId?}");
            });
        }
    }
}
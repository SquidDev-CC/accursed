using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Accursed.Services;
using Accursed.Models;

namespace Accursed
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework().AddSqlite().AddDbContext<AccursedDbContext>();
            services.AddMvc();
            services.AddTransient<IModFetcher, CurseModFetcher>();
            services.AddInstance<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider
                            .GetService<AccursedDbContext>()
                            .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Mod",
                    template: "mods/{slug}",
                    defaults: new { controller = "Mod", action = "ViewMod" }
                );
                routes.MapRoute(
                    name: "Version",
                    template: "mods/{modSlug}/{versionName}",
                    defaults: new { controller = "Mod", action = "ViewVersion" }
                );
                routes.MapRoute(
                    name: "Index",
                    template: "",
                    defaults: new { controller = "Home", action = "Index" }
                );

                // routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    }
}

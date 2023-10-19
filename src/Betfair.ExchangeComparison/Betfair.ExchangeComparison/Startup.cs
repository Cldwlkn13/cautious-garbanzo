using System;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using Betfair.ExchangeComparison.Configurations;
using Betfair.ExchangeComparison.Exchange.Settings;
using Betfair.ExchangeComparison.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Betfair.ExchangeComparison
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.ConfigureExchange();
            services.ConfigureSportsbook();
            services.AddHealthChecks();
            services.AddSignalR();

            services.Configure<LoginSettings>(Configuration.GetSection(nameof(LoginSettings)));

            services.AddDistributedMemoryCache();

            var connectionString = Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);
                //options.Cookie.Name = $"{Domain.Constants.Constants.AppName}";
                //options.Cookie.IsEssential = false;
            });

            BindSettings(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHealthChecks("/health");
            var wwwrootPath = Path.Combine(env.ContentRootPath, "wwwroot");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(wwwrootPath),
                RequestPath = ""
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            //app.UseHttpsRedirection();



            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=SpecialsBuilder}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }

        private void BindSettings(IServiceCollection services)
        {
            services.Configure<ExchangeSettings>(o =>
                Configuration.GetSection("ExchangeSettings").Bind(o));
        }
    }
}


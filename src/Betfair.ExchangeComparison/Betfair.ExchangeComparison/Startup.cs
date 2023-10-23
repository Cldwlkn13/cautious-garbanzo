using Betfair.ExchangeComparison.Configurations;
using Betfair.ExchangeComparison.Data;
using Betfair.ExchangeComparison.Exchange.Settings;
using Betfair.ExchangeComparison.Handlers;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Scraping.Settings;
using Betfair.ExchangeComparison.Services;
using Betfair.ExchangeComparison.Sportsbook.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
            services.ConfigureScrapers();
            services.AddHealthChecks();
            services.AddSignalR();

            services.AddSingleton<ICatalogService, CatalogService>();
            services.AddSingleton<IScrapingOrchestrator, ScrapingOrchestrator>();
            services.AddSingleton<IScrapingControl, ScrapingOrchestrator>();
            services.AddSingleton<IPricingComparisonHandler, PriceComparisonHandler>();
            services.AddSingleton<IMappingService, MappingService>();

            services.Configure<LoginSettings>(Configuration.GetSection(nameof(LoginSettings)));
            services.Configure<ScrapingSettings>(Configuration.GetSection(nameof(ScrapingSettings)));

            services.AddDistributedMemoryCache();

            var connectionString = Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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

            services.AddHostedService<Worker>();

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

            services.Configure<SportsbookSettings>(o =>
               Configuration.GetSection("SportsbookSettings").Bind(o));
        }
    }
}


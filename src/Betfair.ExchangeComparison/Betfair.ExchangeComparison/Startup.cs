using Betfair.ExchangeComparison.Auth.Settings;
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
using Betfair.ExchangeComparison.Workers;
using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Processors;
using Betfair.ExchangeComparison.Settings;

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

            //projects
            services.ConfigureExchange();
            services.ConfigureSportsbook();
            services.ConfigureScrapers();
            services.ConfigureAuth();
            services.AddHealthChecks();
            services.AddSignalR();

            //compound services
            services.AddSingleton<ICatalogService, CatalogService>();

            //processors
            services.AddSingleton<ICatalogProcessor, CatalogProcessor>();
            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddSingleton<IMarketProcessor, MarketProcessor>();
            services.AddSingleton<IRunnerProcessor, RunnerProcessor>();

            services.AddSingleton<ScrapingProcessorFootball>();
            services.AddSingleton<ScrapingProcessorRacing>();

            //scraping
            services.AddSingleton<IScrapingOrchestratorFootball, ScrapingOrchestratorFootball>();
            services.AddSingleton<IScrapingControlFootball, ScrapingOrchestratorFootball>();
            services.AddSingleton<IScrapingOrchestratorRacing, ScrapingOrchestratorRacing>();
            services.AddSingleton<IScrapingControlRacing, ScrapingOrchestratorRacing>();

            //mapping and comparison
            services.AddSingleton<IPricingComparisonHandler, PriceComparisonHandler>();
            services.AddSingleton<IMappingService, MappingService>();

            //settings
            services.Configure<LoginSettings>(Configuration.GetSection(nameof(LoginSettings)));
            services.Configure<ScrapingSettings>(Configuration.GetSection(nameof(ScrapingSettings)));
            services.Configure<PriceComparisonSettings>(Configuration.GetSection(nameof(PriceComparisonSettings)));

            services.AddDistributedMemoryCache();

            var connectionString = Configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);
                //options.Cookie.Name = $"{Domain.Constants.Constants.AppName}";
                //options.Cookie.IsEssential = false;
            });

            services.AddHostedService<RacingWorker>();
            services.AddHostedService<FootballWorker>();

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


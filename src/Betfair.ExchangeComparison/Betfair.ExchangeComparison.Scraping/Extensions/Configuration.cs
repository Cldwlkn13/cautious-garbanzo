using Betfair.ExchangeComparison.Scraping;
using Betfair.ExchangeComparison.Scraping.Boylesports;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Betfair.ExchangeComparison.Scraping.WilliamHill;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureScrapers(this IServiceCollection services)
        {
            services.AddTransient<IScrapingClient, ScrapingClient>();

            //Boylesports
            services.AddSingleton<IBoylesportsHandler, BoylesportsHandler>();
            services.AddSingleton<IBoylesportsParser, BoylesportsParser>();

            //William Hill
            services.AddSingleton<IWilliamHillHandler, WilliamHillHandler>();
            services.AddSingleton<IWilliamHillParser, WilliamHillParser>();

            //Oddschecker
            services.AddSingleton<IOddscheckerHandler, OddscheckerHandler>();
            services.AddSingleton<IOddscheckerParser, OddscheckerParser>();
        }
    }
}


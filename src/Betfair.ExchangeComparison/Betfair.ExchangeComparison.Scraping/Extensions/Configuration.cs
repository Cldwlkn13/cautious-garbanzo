using Betfair.ExchangeComparison.Scraping;
using Betfair.ExchangeComparison.Scraping.Boylesports;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
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
        }
    }
}


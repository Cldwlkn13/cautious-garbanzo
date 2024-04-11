using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Scraping;
using Betfair.ExchangeComparison.Scraping.Boylesports;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Football;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Racing;
using Betfair.ExchangeComparison.Scraping.WilliamHill;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Football;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using Betfair.ExchangeComparison.Scraping.Zenrows;
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureScrapers(this IServiceCollection services)
        {
            services.AddTransient<IScrapingClient, ScrapingClient>();
            services.AddSingleton<IUsageHandler, UsageHandler>();

            //Boylesports
            services.AddSingleton<IBoylesportsHandler, BoylesportsHandler>();
            services.AddSingleton<IBoylesportsParser, BoylesportsParser>();

            //William Hill
            services.AddSingleton<IWilliamHillHandlerFootball, WilliamHillHandlerFootball>();
            services.AddSingleton<IWilliamHillParserFootball, WilliamHillParserFootball>();

            //Oddschecker
            services.AddSingleton<IOddscheckerHandlerRacing, OddscheckerHandlerRacing>();
            services.AddSingleton<IOddscheckerParserRacing, OddscheckerParserRacing>();

            services.AddSingleton<IOddscheckerHandlerFootball, OddscheckerHandlerFootball>();
            services.AddSingleton<IOddscheckerParserFootball, OddscheckerParserFootball>();


        }
    }
}


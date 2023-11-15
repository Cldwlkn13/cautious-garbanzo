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
using Microsoft.Extensions.DependencyInjection;

namespace Betfair.ExchangeComparison.Configurations
{
    public static partial class Configuration
    {
        public static void ConfigureScrapers(this IServiceCollection services)
        {
            services.AddTransient<IScrapingClient, ScrapingClient>();

            //Boylesports
            services.AddSingleton<IBoylesportsHandler<SportRacing>, BoylesportsHandler<SportRacing>>();
            services.AddSingleton<IBoylesportsParser<SportRacing>, BoylesportsParser<SportRacing>>();

            services.AddSingleton<IBoylesportsHandler<SportFootball>, BoylesportsHandler<SportFootball>>();
            services.AddSingleton<IBoylesportsParser<SportFootball>, BoylesportsParser<SportFootball>>();

            //William Hill
            services.AddSingleton<IWilliamHillHandler<SportRacing>, WilliamHillHandlerRacing<SportRacing>>();
            services.AddSingleton<IWilliamHillParser<SportRacing>, WilliamHillParserRacing<SportRacing>>();

            services.AddSingleton<IWilliamHillHandler<SportFootball>, WilliamHillHandlerFootball<SportFootball>>();
            services.AddSingleton<IWilliamHillParser<SportFootball>, WilliamHillParserFootball<SportFootball>>();

            //Oddschecker
            services.AddSingleton<IOddscheckerHandler<SportRacing>, OddscheckerHandlerRacing<SportRacing>>();
            services.AddSingleton<IOddscheckerParser<SportRacing>, OddscheckerParserRacing<SportRacing>>();

            services.AddSingleton<IOddscheckerHandler<SportFootball>, OddscheckerHandlerFootball<SportFootball>>();
            services.AddSingleton<IOddscheckerParser<SportFootball>, OddscheckerParserFootball<SportFootball>>();


        }
    }
}


using Betfair.ExchangeComparison.Domain.Definitions.Base;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker
{
    public class OddscheckerHandler<T> : ScrapingHandler<T>
    {
        public OddscheckerHandler(ILogger<OddscheckerHandler<T>> logger, IScrapingClient scrapingClient) :
            base(logger, scrapingClient)
        {
        }
    }
}


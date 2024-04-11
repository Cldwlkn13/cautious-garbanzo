using Betfair.ExchangeComparison.Scraping.Interfaces;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping
{
    public class ScrapingHandler
    {
        protected readonly ILogger<ScrapingHandler> _logger;
        protected readonly IScrapingClient _scrapingClient;

        public ScrapingHandler(ILogger<ScrapingHandler> logger, IScrapingClient scrapingClient)
        {
            _logger = logger;
            _scrapingClient = scrapingClient;
        }      
    }
}


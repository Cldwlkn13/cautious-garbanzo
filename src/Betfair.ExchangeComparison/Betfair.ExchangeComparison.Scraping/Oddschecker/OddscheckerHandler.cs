using Betfair.ExchangeComparison.Scraping.Interfaces;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker
{
    public class OddscheckerHandler
    {
        protected readonly ILogger<OddscheckerHandler> _logger;
        protected readonly IScrapingClient _scrapingClient;

        public OddscheckerHandler(ILogger<OddscheckerHandler> logger, IScrapingClient scrapingClient) 
        {
            _logger = logger;
            _scrapingClient = scrapingClient;
        }
    }
}


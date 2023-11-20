using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;

namespace Betfair.ExchangeComparison.Scraping.Zenrows
{
    public class UsageHandler : IUsageHandler
    {
        private readonly IScrapingClient _scrapingClient;

        public UsageHandler(IScrapingClient scrapingClient)
        { 
            _scrapingClient = scrapingClient;   
        }

        public async Task<UsageModel> GetUsage() => await _scrapingClient.ZenRowsUsage();
    }
}

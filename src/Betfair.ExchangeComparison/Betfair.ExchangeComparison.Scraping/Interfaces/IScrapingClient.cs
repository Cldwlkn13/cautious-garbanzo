using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingClient
    {
        string Scrape(string url);
        Task<string> ScrapeAsync(string url, bool jsRender = false, bool antiBot = false);
        Task<UsageModel> Usage();
    }
}


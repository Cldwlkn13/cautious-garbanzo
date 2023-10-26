using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingClient
    {
        string ScrapeZenRows(string url);
        Task<string> ScrapeZenRowsAsync(string url, Dictionary<string, string> parameters);
        Task<string> ScrapeAsync(string url);
        Task<UsageModel> ZenRowsUsage();
    }
}


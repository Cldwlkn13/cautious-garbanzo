using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IUsageHandler
    {
        Task<UsageModel> GetUsage();
    }
}

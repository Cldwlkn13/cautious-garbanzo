using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces
{
    public interface IUsageModel
    {
        Task<UsageModel> Usage();
    }
}

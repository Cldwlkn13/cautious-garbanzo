using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces
{
    public interface IBoylesportsParser
    {
        ScrapedEvent BuildScrapedEvent(string html);
        ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEwc compoundObj);
    }
}


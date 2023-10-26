using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IHtmlParser
    {
        ScrapedEvent BuildScrapedEvent(string html);
        ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj);
    }
}


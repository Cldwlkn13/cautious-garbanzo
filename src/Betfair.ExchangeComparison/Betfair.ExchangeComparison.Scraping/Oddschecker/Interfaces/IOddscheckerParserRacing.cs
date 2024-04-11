using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces
{
    public interface IOddscheckerParserRacing 
    {
        ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEwc marketDetailsWithEwc);
    }
}

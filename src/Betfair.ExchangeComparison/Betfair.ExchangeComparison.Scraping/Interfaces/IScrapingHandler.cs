using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingHandler
    {
        Task<ScrapedEvent> Handle(MarketDetailWithEvent @event);
        Task Usage();
    }
}


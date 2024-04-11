using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingHandlerSingle
    {
        Task<ScrapedEvent> Handle(MarketDetailWithEwc @event, Sport sport = Sport.Racing);
    }
}

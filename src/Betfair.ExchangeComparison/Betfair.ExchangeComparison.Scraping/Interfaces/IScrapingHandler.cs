using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingHandler
    {
        Task<ScrapedEvent> Handle(Sport sport = Sport.Racing);
        Task<ScrapedEvent> Handle(MarketDetailWithEvent @event, Sport sport = Sport.Racing);
        Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing);
        Task<UsageModel> Usage();
    }
}


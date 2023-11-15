using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingHandler<T>
    {
        Task<ScrapedEvent> Handle(Sport sport = Sport.Racing);
        Task<ScrapedEvent> Handle(MarketDetailWithEvent @event, Sport sport = Sport.Racing);
        Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing);
        Task<IEnumerable<ScrapedEvent>> HandleEnumerable(IEnumerable<MarketDetailWithEvent> @events, Sport sport = Sport.Racing);
        Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Dictionary<EventWithCompetition, List<MarketDetail>> @events, Sport sport = Sport.Racing);
        Task<IEnumerable<ScrapedEvent>> HandleEnumerable(EventsByCountry ebc);
        Task<UsageModel> Usage();
    }
}

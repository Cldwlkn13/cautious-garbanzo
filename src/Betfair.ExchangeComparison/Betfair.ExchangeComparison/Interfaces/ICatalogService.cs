using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface ICatalogService
    {
        Task<SportsbookCatalogue> GetSportsbookCatalogue(Sport sport, TimeRange? timeRange = null);
        Task<ExchangeCatalogue> GetExchangeCatalogue(Sport sport, TimeRange? timeRange = null);

        IEnumerable<MarketDetailWithEvent> GetCatalog(Sport sport);
        IEnumerable<MarketDetailWithEvent> UpdateCatalog(Sport sport);

        IDictionary<string, Event> GetExchangeEventsWithMarkets(string eventTypeId);
        IEnumerable<MarketCatalogue> GetExchangeMarketCatalogues(string eventTypeId, IEnumerable<string>? eventIds = null);
        ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> GetExchangeMarketBooks(
            IEnumerable<MarketCatalogue> marketCatalogues, IDictionary<string, Event>? eventDict);
        IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> GetSportsbookEventsWithMarkets(string eventTypeId, TimeRange? timeRange = null);
        Dictionary<EventWithCompetition, IEnumerable<MarketDetail>> GetSportsbookEventsWithPrices(IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> eventsWithMarkets);

        public Dictionary<DateTime, Dictionary<Sport, Dictionary<string, Event>>> ExchangeEventStore { get; }
        public Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketCatalogue>>> ExchangeMarketCatalogueStore { get; }

        public Dictionary<DateTime, Dictionary<Sport, IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>>> SportsbookMarketCatalogueStore { get; }
        public Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketDetailWithEvent>>> SportsbookMarketDetailsStore { get; }
    }
}


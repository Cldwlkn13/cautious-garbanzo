using System;
using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface ICatalogService
    {
        IEnumerable<CompoundEventWithMarketDetail> GetCompoundCatalog(Sport sport);
        IEnumerable<CompoundEventWithMarketDetail> UpdateCompoundCatalog(Sport sport);
        IDictionary<string, Event> GetExchangeEventsWithMarkets(string eventTypeId);
        IEnumerable<MarketCatalogue> GetExchangeMarketCatalogues(string eventTypeId, IEnumerable<string>? eventIds = null);
        ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> GetExchangeMarketBooks(
            IEnumerable<MarketCatalogue> marketCatalogues, IDictionary<string, Event>? eventDict);
        IDictionary<Event, IList<MarketCatalogue>> GetSportsbookEventsWithMarkets(string eventTypeId);
        Dictionary<Event, IList<MarketDetail>> GetSportsbookEventsWithPrices(IDictionary<Event, IList<MarketCatalogue>> eventsWithMarkets);

        public Dictionary<DateTime, IEnumerable<CompoundEventWithMarketDetail>> CompoundCatalog { get; }
    }
}


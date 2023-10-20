using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IExchangeHandler _exchangeHandler;
        private readonly ISportsbookHandler _sportsbookHandler;

        public Dictionary<DateTime, IEnumerable<CompoundEventWithMarketDetail>> CompoundCatalog { get; private set; }

        public CatalogService(IExchangeHandler exchangeHandler, ISportsbookHandler sportsbookHandler)
        {
            _exchangeHandler = exchangeHandler;
            _sportsbookHandler = sportsbookHandler;

            CompoundCatalog = new Dictionary<DateTime, IEnumerable<CompoundEventWithMarketDetail>>();
        }

        public IEnumerable<CompoundEventWithMarketDetail> GetCompoundCatalog(Sport sport)
        {
            if (!CompoundCatalog.ContainsKey(DateTime.Today))
            {
                var sportsbookEvents = GetSportsbookEventsWithMarkets(SportMap(sport));
                var sportsbookPrices = GetSportsbookEventsWithPrices(sportsbookEvents);
                var compoundCatalog = BuildCompoundCatalog(sportsbookPrices);

                CompoundCatalog.Add(DateTime.Today, compoundCatalog);

                return compoundCatalog;
            }

            return CompoundCatalog[DateTime.Today];
        }

        public IEnumerable<CompoundEventWithMarketDetail> UpdateCompoundCatalog(Sport sport)
        {
            if (CompoundCatalog.ContainsKey(DateTime.Today))
            {
                var sportsbookEvents = GetSportsbookEventsWithMarkets(SportMap(sport));
                var sportsbookPrices = GetSportsbookEventsWithPrices(sportsbookEvents);
                var compoundCatalog = BuildCompoundCatalog(sportsbookPrices);

                CompoundCatalog[DateTime.Today] = compoundCatalog;

                return compoundCatalog;
            }
            else
            {
                GetCompoundCatalog(sport);
            }

            return CompoundCatalog[DateTime.Today];
        }

        public IDictionary<string, Event> GetExchangeEventsWithMarkets(string eventTypeId)
        {
            try
            {
                if (!_exchangeHandler.SessionValid())
                {
                    var login = _exchangeHandler.Login("", "");
                }

                var marketCatalogues = GetExchangeMarketCatalogues(eventTypeId);

                var eventDict = new Dictionary<string, Event>();
                var eventsOnly = marketCatalogues.Select(m => m.Event);
                foreach (var @event in eventsOnly)
                {
                    if (!eventDict.ContainsKey(@event.Id))
                    {
                        eventDict.Add(@event.Id, @event);
                    }
                }

                return eventDict;
            }
            catch (APINGException exception)
            {
                Console.WriteLine($"APING_EXCEPTION; CatalogService : GetExchangeEventsWithMarkets; Exception={exception.Message}");

                return new Dictionary<string, Event>();
            }
        }

        public IEnumerable<MarketCatalogue> GetExchangeMarketCatalogues(string eventTypeId)
        {
            try
            {
                return _exchangeHandler.ListMarketCatalogues(eventTypeId);
            }
            catch (APINGException exception)
            {
                Console.WriteLine($"APING_EXCEPTION; CatalogService : ListMarketCatalogues; Exception={exception.Message}");

                return new List<MarketCatalogue>();
            }
        }

        public Dictionary<Event, Dictionary<DateTime, IList<MarketBook>>> GetExchangeMarketBooks(
            IEnumerable<MarketCatalogue> marketCatalogues, IDictionary<string, Event>? eventDict)
        {
            var marketBooks = new Dictionary<Event, Dictionary<DateTime, IList<MarketBook>>>();
            foreach (var @event in marketCatalogues.GroupBy(m => m.Event.Id))
            {
                var marketsInEvent = @event.GroupBy(m => m.Description.MarketTime).ToList();
                var marketBooksInEvent = new Dictionary<DateTime, IList<MarketBook>>();

                Parallel.ForEach(marketsInEvent, market =>
                {
                    //foreach (var market in marketsInEvent)
                    //{
                    var marketIdsInRace = market.Select(m => m.MarketId);
                    var batchResult = _exchangeHandler.ListMarketBooks(marketIdsInRace.ToList());
                    marketBooksInEvent.Add(market.Key, batchResult);
                    //}
                });

                marketBooks.Add(eventDict![@event.Key], marketBooksInEvent);
            }

            return marketBooks;
        }

        public IDictionary<Event, IList<MarketCatalogue>> GetSportsbookEventsWithMarkets(string eventTypeId)
        {
            if (!_sportsbookHandler.SessionValid())
            {
                var sbklogin = _sportsbookHandler.Login("", "");
            }

            var events = _sportsbookHandler.ListEventsByEventType(eventTypeId).Select(e => e.Event);

            var eventIds = events.Where(e =>
                e.CountryCode == "GB" ||
                e.CountryCode == "IE")
                    .Select(e => e.Id)
                    .ToHashSet();

            var eventsWithMarkets = new ConcurrentDictionary<Event, IList<MarketCatalogue>>();
            Parallel.ForEach(eventIds, eventId =>
            {
                //foreach (var eventId in eventIds)
                //{
                var marketCatalogue = _sportsbookHandler.ListMarketCatalogues(new HashSet<string> { eventId });

                eventsWithMarkets.AddOrUpdate(events.First(e => e.Id == eventId),
                    marketCatalogue,
                    (k, v) => v = marketCatalogue);
                //}
            });

            return eventsWithMarkets;
        }

        public Dictionary<Event, IList<MarketDetail>> GetSportsbookEventsWithPrices(IDictionary<Event, IList<MarketCatalogue>> eventsWithMarkets)
        {
            var marketIds = eventsWithMarkets.SelectMany(m => m.Value)
                .Select(m => m.MarketId)
                .ToList();

            var prices = _sportsbookHandler.ListPrices(marketIds);

            var eventsWithPrices = new Dictionary<Event, IList<MarketDetail>>();
            foreach (var eventResult in eventsWithMarkets)
            {
                if (eventResult.Key.Name.ToLower().Contains("odds") ||
                    eventResult.Key.Name.ToLower().Contains("specials"))
                {
                    continue;
                }

                var marketsInEvent = new List<MarketDetail>();

                foreach (var marketCatalog in eventResult.Value)
                {
                    var marketDetail = prices.marketDetails.FirstOrDefault(m => m.marketId == marketCatalog.MarketId);

                    if (marketDetail != null)
                    {
                        marketsInEvent.Add(marketDetail);
                    }
                }

                eventsWithPrices.Add(eventResult.Key, marketsInEvent.OrderBy(m => m.marketStartTime).ToList());
            }

            return eventsWithPrices;
        }

        public IList<CompoundEventWithMarketDetail> BuildCompoundCatalog(Dictionary<Event, IList<MarketDetail>> eventCatalog)
        {
            var result = new List<CompoundEventWithMarketDetail>();

            foreach (var @event in eventCatalog)
            {
                if (!@event.Value.Any()) continue;

                foreach (var market in @event.Value)
                {
                    result.Add(new CompoundEventWithMarketDetail { Event = @event.Key, SportsbookMarket = market });
                }
            }

            return result;
        }

        private static string SportMap(Sport sport)
        {
            switch (sport)
            {
                case Sport.Racing:
                    return "7";
                case Sport.Football:
                    return "1";
            }

            return "0";
        }
    }
}


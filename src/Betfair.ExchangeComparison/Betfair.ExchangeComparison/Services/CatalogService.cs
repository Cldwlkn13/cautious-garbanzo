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

        public Dictionary<DateTime, IEnumerable<MarketDetailWithEvent>> Catalog { get; private set; }

        public CatalogService(IExchangeHandler exchangeHandler, ISportsbookHandler sportsbookHandler)
        {
            _exchangeHandler = exchangeHandler;
            _sportsbookHandler = sportsbookHandler;

            Catalog = new Dictionary<DateTime, IEnumerable<MarketDetailWithEvent>>();
        }

        public Task<SportsbookCatalogue> GetSportsbookCatalogue(Sport sport)
        {
            var sportsbookEvents = GetSportsbookEventsWithMarkets(SportMap(sport));
            var sportsbookPrices = GetSportsbookEventsWithPrices(sportsbookEvents);

            var sportsbookCatalogue = new SportsbookCatalogue()
            {
                EventsWithMarketCatalogue = sportsbookEvents,
                EventsWithMarketDetails = sportsbookPrices
            };

            return Task.FromResult(sportsbookCatalogue);
        }

        public Task<ExchangeCatalogue> GetExchangeCatalogue(Sport sport)
        {
            var eventDictionary = GetExchangeEventsWithMarkets(SportMap(sport));
            var marketCatalogues = GetExchangeMarketCatalogues(SportMap(sport));
            var marketBooks = GetExchangeMarketBooks(marketCatalogues, eventDictionary);

            var exchangeCatalogue = new ExchangeCatalogue()
            {
                EventDictionary = eventDictionary,
                MarketCatalogues = marketCatalogues,
                MarketBooks = marketBooks
            };

            return Task.FromResult(exchangeCatalogue);
        }

        public IEnumerable<MarketDetailWithEvent> GetCatalog(Sport sport)
        {
            if (!Catalog.ContainsKey(DateTime.Today))
            {
                var sportsbookEvents = GetSportsbookEventsWithMarkets(SportMap(sport));
                var sportsbookPrices = GetSportsbookEventsWithPrices(sportsbookEvents);
                var compoundCatalog = BuildCompoundCatalog(sportsbookPrices);

                Catalog.Add(DateTime.Today, compoundCatalog);

                return compoundCatalog;
            }

            return Catalog[DateTime.Today];
        }

        public IEnumerable<MarketDetailWithEvent> UpdateCatalog(Sport sport)
        {
            if (Catalog.ContainsKey(DateTime.Today))
            {
                var sportsbookEvents = GetSportsbookEventsWithMarkets(SportMap(sport));
                var sportsbookPrices = GetSportsbookEventsWithPrices(sportsbookEvents);
                var compoundCatalog = BuildCompoundCatalog(sportsbookPrices);

                Catalog[DateTime.Today] = compoundCatalog;

                return compoundCatalog;
            }
            else
            {
                GetCatalog(sport);
            }

            return Catalog[DateTime.Today];
        }

        public IDictionary<string, Event> GetExchangeEventsWithMarkets(string eventTypeId)
        {
            try
            {
                if (!_exchangeHandler.SessionValid())
                {
                    var login = _exchangeHandler.Login("", "");
                }

                var events = _exchangeHandler.ListEvents(eventTypeId);

                var marketCatalogues = GetExchangeMarketCatalogues(eventTypeId, events.Select(e => e.Event.Id));

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
                Console.WriteLine($"APING_EXCEPTION; " +
                    $"CatalogService : GetExchangeEventsWithMarkets(); " +
                    $"Exception={exception.Message}; " +
                    $"ErrorCode={exception.ErrorCode}; " +
                    $"ErrorDetails={exception.ErrorDetails};");

                return new Dictionary<string, Event>();
            }
        }

        public IEnumerable<MarketCatalogue> GetExchangeMarketCatalogues(string eventTypeId, IEnumerable<string>? eventIds = null)
        {
            try
            {
                List<MarketCatalogue> result = new List<MarketCatalogue>();

                switch (eventTypeId)
                {
                    case "7":
                        result = _exchangeHandler.ListMarketCatalogues(eventTypeId).ToList();
                        break;
                    case "1":
                        foreach (var batch in eventIds.Chunk(20))
                        {
                            var batchedCatalogues = _exchangeHandler.ListMarketCatalogues(eventTypeId, null, batch).ToList();
                            result.AddRange(batchedCatalogues);
                        }
                        break;
                }

                return result;
            }
            catch (APINGException exception)
            {
                Console.WriteLine($"APING_EXCEPTION; " +
                    $"CatalogService : ListMarketCatalogues(); " +
                    $"Exception={exception.Message}; " +
                    $"ErrorCode={exception.ErrorCode}; " +
                    $"ErrorDetails={exception.ErrorDetails};");

                return new List<MarketCatalogue>();
            }
        }

        public ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> GetExchangeMarketBooks(
            IEnumerable<MarketCatalogue> marketCatalogues, IDictionary<string, Event>? eventDict)
        {
            var marketBooks = new ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>>();
            var eventsGroupedByEventId = marketCatalogues.GroupBy(m => m.Event.Id);

            Parallel.ForEach(eventsGroupedByEventId, @event =>
            {
                var marketsInEvent = @event.GroupBy(m => m.Description.MarketTime).ToList();
                var marketBooksInEvent = new ConcurrentDictionary<DateTime, IList<MarketBook>>();

                Parallel.ForEach(marketsInEvent, market =>
                {
                    var marketIdsInEvent = market.Select(m => m.MarketId);
                    var batchResult = _exchangeHandler.ListMarketBooks(marketIdsInEvent.ToList());
                    marketBooksInEvent.AddOrUpdate(market.Key, batchResult, (k, v) => v = batchResult);
                });

                marketBooks.AddOrUpdate(eventDict![@event.Key], marketBooksInEvent, (k, v) => v = marketBooksInEvent);

            });

            return marketBooks;
        }

        public IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> GetSportsbookEventsWithMarkets(string eventTypeId)
        {
            if (!_sportsbookHandler.SessionValid())
            {
                var sbklogin = _sportsbookHandler.Login("", "");
            }

            IEnumerable<Event> events = new List<Event>();
            Dictionary<Competition, List<Event>> eventsByCompetition = new Dictionary<Competition, List<Event>>();
            switch (eventTypeId)
            {
                case "7":
                    events = _sportsbookHandler.ListEventsByEventType(eventTypeId);
                    eventsByCompetition.Add(new Competition(), events.ToList());
                    break;
                case "1":
                    var competitions = _sportsbookHandler.ListCompetitions(eventTypeId);
                    eventsByCompetition = _sportsbookHandler.ListEventsByCompetition(
                        eventTypeId, competitions.Select(c => c.Competition));
                    events = eventsByCompetition.SelectMany(e => e.Value);
                    break;
            }

            var eventsWithMarkets = new ConcurrentDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>();
            Parallel.ForEach(eventsByCompetition, eventByCompetition =>
            {
                var eventIds = new HashSet<string>();
                switch (eventTypeId)
                {
                    case "7":
                        eventIds = eventByCompetition.Value
                            .Where(e =>
                                e.CountryCode == "GB" ||
                                e.CountryCode == "IE")
                                .Select(e => e.Id)
                                .ToHashSet();
                        break;
                    case "1":
                        eventIds = eventByCompetition.Value
                            .Select(e => e.Id)
                            .ToHashSet();
                        break;
                }

                foreach (var eventId in eventIds)
                {
                    var marketCatalogue = _sportsbookHandler.ListMarketCatalogues(
                        new HashSet<string>
                        {
                            eventId
                        }, eventTypeId);

                    eventsWithMarkets.AddOrUpdate(new EventWithCompetition
                    {
                        Event = eventByCompetition.Value.First(e => e.Id == eventId),
                        Competition = eventByCompetition.Key
                    },
                        marketCatalogue,
                        (k, v) => v = marketCatalogue);
                }
            });

            return eventsWithMarkets;
        }

        public Dictionary<EventWithCompetition, IEnumerable<MarketDetail>> GetSportsbookEventsWithPrices(IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> eventsWithMarkets)
        {
            var result = new Dictionary<EventWithCompetition, IEnumerable<MarketDetail>>();

            var marketIds = eventsWithMarkets.SelectMany(m => m.Value)
                .Select(m => m.MarketId)
                .ToList();

            if (!marketIds.Any())
            {
                return result;
            }

            var prices = _sportsbookHandler.ListPrices(marketIds);

            foreach (var eventwithCompetition in eventsWithMarkets)
            {
                if (eventwithCompetition.Key.Event.Name.ToLower().Contains("odds") ||
                    eventwithCompetition.Key.Event.Name.ToLower().Contains("specials"))
                {
                    continue;
                }

                var marketsInEvent = new List<MarketDetail>();

                foreach (var marketCatalog in eventwithCompetition.Value)
                {
                    var marketDetail = prices.marketDetails.FirstOrDefault(m => m.marketId == marketCatalog.MarketId);

                    if (marketDetail != null)
                    {
                        marketsInEvent.Add(marketDetail);
                    }
                }

                result.Add(eventwithCompetition.Key, marketsInEvent.OrderBy(m => m.marketStartTime).ToList());
            }

            return result;
        }

        public IList<MarketDetailWithEvent> BuildCompoundCatalog(Dictionary<EventWithCompetition, IEnumerable<MarketDetail>> eventCatalog)
        {
            var result = new List<MarketDetailWithEvent>();

            foreach (var @event in eventCatalog)
            {
                if (!@event.Value.Any()) continue;

                foreach (var market in @event.Value)
                {
                    result.Add(new MarketDetailWithEvent { EventWithCompetition = @event.Key, SportsbookMarket = market });
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


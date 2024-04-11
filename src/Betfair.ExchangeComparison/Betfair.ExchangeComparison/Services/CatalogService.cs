using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Sport = Betfair.ExchangeComparison.Domain.Enums.Sport;

namespace Betfair.ExchangeComparison.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IExchangeHandler _exchangeHandler;
        private readonly IBetfairSportsbookHandler _bfSportsbookHandler;
        private readonly IPaddyPowerSportsbookHandler _ppSportsbookHandler;
        private readonly IMatchbookHandler _matchbookHandler;

        public Dictionary<DateTime, Dictionary<Sport, Dictionary<string, Event>>> ExchangeEventStore { get; set; }
        public Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketCatalogue>>> ExchangeMarketCatalogueStore { get; set; }

        public Dictionary<DateTime, Dictionary<Sport, IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>>> SportsbookMarketCatalogueStore { get; private set; }
        public Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketDetailWithEwc>>> SportsbookMarketDetailsStore { get; private set; }

        public CatalogService(IExchangeHandler exchangeHandler, IBetfairSportsbookHandler bfSportsbookHandler, 
            IPaddyPowerSportsbookHandler ppSportsbookHandler, IMatchbookHandler matchbookHandler)
        {
            _exchangeHandler = exchangeHandler;
            _bfSportsbookHandler = bfSportsbookHandler;
            _ppSportsbookHandler = ppSportsbookHandler;
            _matchbookHandler = matchbookHandler;

            ExchangeEventStore = new Dictionary<DateTime, Dictionary<Sport, Dictionary<string, Event>>>();
            ExchangeMarketCatalogueStore = new Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketCatalogue>>>();

            SportsbookMarketCatalogueStore = new Dictionary<DateTime, Dictionary<Sport, IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>>>();
            SportsbookMarketDetailsStore = new Dictionary<DateTime, Dictionary<Sport, IEnumerable<MarketDetailWithEwc>>>();
        }

        public Task<SportsbookCatalogue> GetSportsbookCatalogue(
            Sport sport, TimeRange? timeRange = null, 
            Bookmaker bookmaker = Bookmaker.BetfairSportsbook, int addDays = 1)
        {
            var handler = ResolveHandler(bookmaker);

            handler.TryLogin();

            var sportsbookEventsToday = SportsbookMarketCataloguesToday(sport, addDays);
            var sportsbookMarketDetails = GetSportsbookEventsWithPrices(sportsbookEventsToday);

            var sportsbookCatalogue = new SportsbookCatalogue()
            {
                EventsWithMarketCatalogue = sportsbookEventsToday,
                EventsWithMarketDetails = sportsbookMarketDetails
            };

            return Task.FromResult(sportsbookCatalogue);
        }

        public async Task<ExchangeCatalogue> GetExchangeCatalogue(Sport sport, TimeRange? timeRange = null)
        {
            if (!_exchangeHandler.TryLogin())
            {
                Console.WriteLine($"LOGIN_FAILED");
                return new ExchangeCatalogue();
            }

            TimeRange time = BetfairQueryExtensions.TimeRangeForNextDays(1);
            if (timeRange != null)
            {
                time = timeRange;
            }

            Dictionary<string, Event> events = ExchangeEventsToday(sport)
                .Where(e => e.Value.OpenDate <= time.To)
                .ToDictionary(x => x.Key, v => v.Value);

            IEnumerable<MarketCatalogue> marketCatalogues = ExchangeMarketCataloguesToday(
                sport, events.Select(e => e.Key))
                    .Where(e => e.Description.MarketTime <= time.To)
                    .ToList();

            var marketBooks = GetExchangeMarketBooks(marketCatalogues, events);

            var exchangeCatalogue = new ExchangeCatalogue()
            {
                EventDictionary = events,
                MarketCatalogues = marketCatalogues,
                MarketBooks = marketBooks
            };

            return exchangeCatalogue;
        }

        public async Task<List<MatchbookEvent>> GetMatchbookCatalogue(Domain.Enums.Sport sport, TimeRange? timeRange = null)
        {
            await _matchbookHandler.GetSessionToken(true);

            var events = await _matchbookHandler.GetEvents();

            return events;
        }

        public IEnumerable<MarketDetailWithEwc> UpdateMarketDetailCatalog(Sport sport, int addDays = 1)
        {
            var handler = ResolveHandler(Bookmaker.BetfairSportsbook);
            if (!handler.TryLogin())
            {
                Console.WriteLine($"LOGIN_FAILED");
                return new List<MarketDetailWithEwc>();
            }

            if (SportsbookMarketDetailsStore.ContainsKey(DateTime.Today.AddDays(addDays - 1)))
            {
                var sportsbookMarketDetails = SportsbookMarketCataloguesToday(sport, addDays);

                var sportsbooMarketDetails = GetSportsbookEventsWithPrices(sportsbookMarketDetails);

                var marketDetailsWithEvents = ListMarketDetailsWithEvents(sportsbooMarketDetails);

                if (SportsbookMarketDetailsStore[DateTime.Today.AddDays(addDays - 1)].ContainsKey(sport))
                {
                    SportsbookMarketDetailsStore[DateTime.Today.AddDays(addDays - 1)][sport] = marketDetailsWithEvents;
                }

                return marketDetailsWithEvents;
            }
            else
            {
                GetCatalog(sport, addDays);
            }

            return SportsbookMarketDetailsStore[DateTime.Today.AddDays(addDays - 1)][sport];
        }

        public Dictionary<EventWithCompetition, List<MarketDetail>> UpdateMarketDetailCatalogGroupByEvent(Sport sport, int addDays = 1)
        {
            var sportsbookCatalog = UpdateMarketDetailCatalog(sport, addDays);

            var groupedByEventId = sportsbookCatalog.GroupBy(e => e.EventWithCompetition.Event.Name);

            var result = new Dictionary<EventWithCompetition, List<MarketDetail>>();

            foreach (var grp in groupedByEventId)
            {
                if (!result.Any(g => g.Key.Event.Id == grp.Key))
                {
                    result.Add(grp.First().EventWithCompetition, new List<MarketDetail>(grp.Select(m => m.SportsbookMarket).ToList()));
                }
            }

            return result;
        }

        #region helper methods

        public IEnumerable<MarketDetailWithEwc> GetCatalog(Sport sport, int addDays = 1)
        {
            var handler = ResolveHandler(Bookmaker.BetfairSportsbook);
            if (!handler.TryLogin())
            {
                Console.WriteLine($"LOGIN_FAILED");
                return new List<MarketDetailWithEwc>();
            }

            if (!SportsbookMarketDetailsStore.ContainsKey(DateTime.Today.AddDays(addDays - 1)))
            {
                var sportsbookEvents = SportsbookMarketCataloguesToday(sport, addDays);

                var sportsbookPrices = GetSportsbookEventsWithPrices(sportsbookEvents);
                var marketDetailsWithEvents = ListMarketDetailsWithEvents(sportsbookPrices);

                SportsbookMarketDetailsStore.Add(DateTime.Today.AddDays(addDays - 1),
                    new Dictionary<Sport, IEnumerable<MarketDetailWithEwc>>()
                    {
                        { sport, marketDetailsWithEvents }
                    });

                return marketDetailsWithEvents;
            }

            return SportsbookMarketDetailsStore[DateTime.Today.AddDays(addDays - 1)][sport];
        }

        public IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> SportsbookMarketCataloguesToday(Sport sport, int addDays = 1)
        {
            if (!SportsbookMarketCatalogueStore.ContainsKey(DateTime.Today.AddDays(addDays - 1)))
            {
                SportsbookMarketCatalogueStore.Add(DateTime.Today.AddDays(addDays - 1), new Dictionary<Sport, IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>>()
                {
                    { sport , new Dictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>() }
                });

                SportsbookMarketCatalogues();
            }
            else if (!SportsbookMarketCatalogueStore[DateTime.Today.AddDays(addDays - 1)].ContainsKey(sport))
            {
                SportsbookMarketCatalogueStore[DateTime.Today.AddDays(addDays - 1)] = new Dictionary<Sport, IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>>()
                {
                    { sport, new ConcurrentDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>>() }
                };

                SportsbookMarketCatalogues();
            }
            else if (!SportsbookMarketCatalogueStore[DateTime.Today.AddDays(addDays - 1)][sport].Any())
            {
                SportsbookMarketCatalogues();
            }

            void SportsbookMarketCatalogues()
            {
                var sportsbookMarketCataloguesToday = GetSportsbookEventsWithMarkets(
                    sport.SportMap(),
                    BetfairQueryExtensions.TimeRangeForNextDays(addDays));

                SportsbookMarketCatalogueStore[DateTime.Today.AddDays(addDays - 1)][sport] = sportsbookMarketCataloguesToday;
            }

            return SportsbookMarketCatalogueStore[DateTime.Today.AddDays(addDays - 1)][sport];
        }

        public Dictionary<string, Event> ExchangeEventsToday(Sport sport)
        {
            if (!ExchangeEventStore.ContainsKey(DateTime.Today))
            {
                ExchangeEventStore.Add(DateTime.Today, new Dictionary<Sport, Dictionary<string, Event>>()
                {
                    { sport , new Dictionary<string, Event>() }
                });

                ExchangeEvents();
            }
            else if (!ExchangeEventStore[DateTime.Today].ContainsKey(sport))
            {
                ExchangeEventStore[DateTime.Today] = new Dictionary<Sport, Dictionary<string, Event>>
                    {
                        { sport, new Dictionary<string, Event>() }
                    };

                ExchangeEvents();
            }
            else if (!ExchangeEventStore[DateTime.Today][sport].Any())
            {
                ExchangeEvents();
            }

            void ExchangeEvents()
            {
                IEnumerable<Event> exchangeEventsToday = new List<Event>();

                switch (sport)
                {
                    case Sport.Football:
                        var exchangeCompetitionsToday = _exchangeHandler.ListCompetitions(
                            sport.SportMap(),
                            BetfairQueryExtensions.TimeRangeForNextDays(1))
                                .Select(er => er.Competition);
                        var eventsByCompetition = _exchangeHandler.ListEventsByCompetition(
                            exchangeCompetitionsToday,
                            sport.SportMap(),
                            BetfairQueryExtensions.TimeRangeForNextDays(1));
                        exchangeEventsToday = eventsByCompetition.SelectMany(e => e.Value);
                        break;

                    case Sport.Racing:
                        exchangeEventsToday = _exchangeHandler.ListEvents(
                            sport.SportMap(),
                            BetfairQueryExtensions.TimeRangeForNextWholeDays(1))
                                .Select(er => er.Event);
                        break;
                }

                ExchangeEventStore[DateTime.Today][sport] = exchangeEventsToday
                    .ToDictionary(x => x.Id, x => x);
            }

            return ExchangeEventStore[DateTime.Today][sport];
        }

        public IEnumerable<MarketCatalogue> ExchangeMarketCataloguesToday(Sport sport, IEnumerable<string> eventIds)
        {
            if (!ExchangeMarketCatalogueStore.ContainsKey(DateTime.Today))
            {
                ExchangeMarketCatalogueStore.Add(DateTime.Today, new Dictionary<Sport, IEnumerable<MarketCatalogue>>()
                {
                    { sport , new List<MarketCatalogue>() }
                });

                ExchangeMarketCatalogues();
            }
            else if (!ExchangeMarketCatalogueStore[DateTime.Today].ContainsKey(sport))
            {
                ExchangeMarketCatalogueStore[DateTime.Today] = new Dictionary<Sport, IEnumerable<MarketCatalogue>>
                    {
                        { sport, new List<MarketCatalogue>() }
                    };

                ExchangeMarketCatalogues();
            }
            else if (!ExchangeMarketCatalogueStore[DateTime.Today][sport].Any())
            {
                ExchangeMarketCatalogues();
            }

            void ExchangeMarketCatalogues()
            {
                var exchangeMarketCataloguesToday = GetExchangeMarketCatalogues(
                    sport.SportMap(),
                    eventIds);

                ExchangeMarketCatalogueStore[DateTime.Today][sport] = exchangeMarketCataloguesToday
                    .ToList();
            }

            return ExchangeMarketCatalogueStore[DateTime.Today][sport];
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
                        foreach (var batch in eventIds.Chunk(10))
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
                    $"CatalogService : GetExchangeMarketCatalogues(); " +
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

        public IDictionary<EventWithCompetition, IEnumerable<MarketCatalogue>> GetSportsbookEventsWithMarkets(string eventTypeId, TimeRange? timeRange = null)
        {
            var handler = ResolveHandler(Bookmaker.BetfairSportsbook);
            handler.TryLogin();

            IEnumerable<Event> events = new List<Event>();
            Dictionary<Competition, List<Event>> eventsByCompetition = new Dictionary<Competition, List<Event>>();

            var time = new TimeRange();
            if (timeRange == null)
            {
                timeRange = BetfairQueryExtensions.TimeRangeForNextDays(1);
            }
            else
            {
                time = timeRange;
            }

            switch (eventTypeId)
            {
                case "7":
                    events = _bfSportsbookHandler.ListEventsByEventType(eventTypeId, time);
                    eventsByCompetition.Add(new Competition(), events.ToList());
                    break;
                case "1":
                    var competitions = _bfSportsbookHandler.ListCompetitions(eventTypeId, time);
                    eventsByCompetition = _bfSportsbookHandler.ListEventsByCompetition(
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
                    var marketCatalogue = _bfSportsbookHandler.ListMarketCatalogues(
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
            var handler = ResolveHandler(Bookmaker.BetfairSportsbook);
            handler.TryLogin();

            var result = new Dictionary<EventWithCompetition, IEnumerable<MarketDetail>>();

            var marketIds = eventsWithMarkets.SelectMany(m => m.Value)
                .Select(m => m.MarketId)
                .ToList();

            if (!marketIds.Any())
            {
                return result;
            }

            var prices = _bfSportsbookHandler.ListPrices(marketIds);

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
                    var marketDetail = prices.marketDetails
                        .FirstOrDefault(m => m.marketId == marketCatalog.MarketId);

                    if (marketDetail != null && marketDetail.marketStatus == "OPEN" && !marketDetail.inplay)
                    {
                        marketsInEvent.Add(marketDetail);
                    }
                }

                result.Add(eventwithCompetition.Key, marketsInEvent.OrderBy(m => m.marketStartTime).ToList());
            }

            return result;
        }

        public IList<MarketDetailWithEwc> ListMarketDetailsWithEvents(Dictionary<EventWithCompetition, IEnumerable<MarketDetail>> eventCatalog)
        {
            var result = new List<MarketDetailWithEwc>();

            foreach (var @event in eventCatalog)
            {
                if (!@event.Value.Any()) continue;

                foreach (var market in @event.Value)
                {
                    result.Add(new MarketDetailWithEwc
                    {
                        EventWithCompetition = @event.Key,
                        SportsbookMarket = market
                    }
                    );
                }
            }

            return result;
        }

        private ISportsbookHandler ResolveHandler(Bookmaker bookmaker)
        {
            switch (bookmaker)
            {
                case Bookmaker.PaddyPower:
                    return _ppSportsbookHandler;
                default:
                    return _bfSportsbookHandler;
            }
        }

        #endregion
    }
}


using Betfair.ExchangeComparison.Auth.Interfaces;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Clients;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Exchange.Settings;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Exchange
{
    public class ExchangeHandler : IExchangeHandler
    {
        private readonly IOptions<ExchangeSettings> _options;
        private readonly IAuthHandler _authHandler;

        private IExchangeClient? _exchangeClient;
        private const Bookmaker _bookmaker = Bookmaker.BetfairExchange;

        public ExchangeHandler(IOptions<ExchangeSettings> options, IAuthHandler authHandler)
        {
            _options = options;
            _authHandler = authHandler;

            if (_authHandler.TryLogin(_bookmaker))
            {
                _exchangeClient = new ExchangeClient(
                    _options.Value.Url,
                    _authHandler.AppKey,
                    _authHandler.SessionTokens[_bookmaker]);
            }
        }

        public bool TryLogin()
        {
            var isLoginSuccessful = _authHandler.TryLogin(_bookmaker);
            _exchangeClient = new ExchangeClient(
                _options.Value.Url,
                _authHandler.AppKey,
                _authHandler.SessionTokens[_bookmaker]);
            return isLoginSuccessful;
        }

        public bool Login(string username = "", string password = "")
        {
            var isLoginSuccessful = _authHandler.Login(username, password, _bookmaker);
            _exchangeClient = new ExchangeClient(
                _options.Value.Url,
                _authHandler.AppKey,
                _authHandler.SessionTokens[_bookmaker]);
            return isLoginSuccessful;
        }

        public bool SessionValid() =>
            _authHandler.SessionValid(_bookmaker);

        public IList<EventTypeResult> ListEventTypes()
        {
            var marketFilter = new MarketFilter();

            var eventTypes = _exchangeClient?.ListEventTypes(marketFilter) ??
                throw new NullReferenceException($"Event Types null.");

            return eventTypes;
        }

        public IList<CompetitionResult> ListCompetitions
            (string eventTypeId = "7", TimeRange? timeRange = null)
        {
            var time = new TimeRange();

            if (timeRange == null)
            {
                time = new TimeRange()
                {
                    From = DateTime.Now,
                    To = eventTypeId == "7" ?
                    DateTime.Today.AddDays(_options.Value.RacingQueryToDays) :
                    DateTime.Now.AddHours(_options.Value.FootballQueryToHours)
                };
            }
            else
            {
                time = timeRange;
            }

            var marketFilter = new MarketFilter();
            marketFilter.EventTypeIds = new List<string> { eventTypeId }.ToHashSet();
            marketFilter.MarketStartTime = time;
            marketFilter.MarketCountries = eventTypeId.CountryCodes();

            var result = _exchangeClient?.ListCompetitions(marketFilter) ??
                throw new NullReferenceException($"Competitions null.");

            return result;
        }

        public IList<EventResult> ListEvents(
            string eventTypeId = "7", TimeRange? timeRange = null)
        {
            var time = new TimeRange();

            if (timeRange == null)
            {
                time = new TimeRange()
                {
                    From = DateTime.Now,
                    To = eventTypeId == "7" ?
                    DateTime.Today.AddDays(_options.Value.RacingQueryToDays) :
                    DateTime.Now.AddHours(_options.Value.FootballQueryToHours)
                };
            }
            else
            {
                time = timeRange;
            }

            var marketFilter = new MarketFilter();
            marketFilter.EventTypeIds = new List<string> { eventTypeId }.ToHashSet();
            marketFilter.MarketStartTime = time;
            marketFilter.MarketCountries = eventTypeId.CountryCodes();

            var events = _exchangeClient?.ListEvents(marketFilter) ??
                throw new NullReferenceException($"Events null.");

            return events;
        }

        public Dictionary<Competition, IEnumerable<Event>> ListEventsByCompetition(IEnumerable<Competition> competitions,
                string eventTypeId = "7", TimeRange? timeRange = null)
        {
            var result = new Dictionary<Competition, IEnumerable<Event>>();

            var time = new TimeRange();

            if (timeRange == null)
            {
                time = new TimeRange()
                {
                    From = DateTime.Now,
                    To = eventTypeId == "7" ?
                    DateTime.Today.AddDays(_options.Value.RacingQueryToDays) :
                    DateTime.Now.AddHours(_options.Value.FootballQueryToHours)
                };
            }
            else
            {
                time = timeRange;
            }

            foreach (var competition in competitions)
            {
                var marketFilter = new MarketFilter();
                marketFilter.CompetitionIds = new List<string> { competition.Id }.ToHashSet();
                marketFilter.MarketStartTime = time;
                marketFilter.MarketCountries = eventTypeId.CountryCodes();

                var events = _exchangeClient?.ListEvents(marketFilter) ??
                    throw new NullReferenceException($"Events null.");

                result.Add(competition, events.Select(e => e.Event));
            }

            return result;
        }

        public IList<MarketCatalogue> ListMarketCatalogues(
            string eventTypeId = "7", TimeRange? timeRange = null, IEnumerable<string>? eventIds = null)
        {
            var marketFilter = new MarketFilter();
            marketFilter.EventTypeIds = new List<string> { eventTypeId }.ToHashSet();

            if (eventIds != null)
            {
                marketFilter.EventIds = eventIds.ToHashSet();
            }

            var time = new TimeRange();

            if (timeRange == null)
            {
                time.From = DateTime.Now;
                time.To = eventTypeId == "7" ?
                    DateTime.Today.AddDays(_options.Value.RacingQueryToDays) :
                    DateTime.Now.AddHours(_options.Value.FootballQueryToHours);
            }
            else
            {
                time = timeRange;
            }

            
            marketFilter.MarketCountries = eventTypeId.CountryCodes();
            marketFilter.MarketTypeCodes = eventTypeId.MarketTypes();
            var marketSort = MarketSort.FIRST_TO_START;
            var maxResults = "1000";

            //as an example we requested runner metadata 
            ISet<MarketProjection> marketProjections = new HashSet<MarketProjection>();
            marketProjections.Add(MarketProjection.COMPETITION);
            marketProjections.Add(MarketProjection.EVENT);
            marketProjections.Add(MarketProjection.MARKET_DESCRIPTION);

            var result = new List<MarketCatalogue>();

            for(int i = 0; i < 12; i++)
            {
                var t = new TimeRange();
                t.From = DateTime.Now.AddHours(i);
                t.To = DateTime.Now.AddHours(i + 1);

                marketFilter.MarketStartTime = t;

                var marketCatalogues = _exchangeClient!.ListMarketCatalogue(
                     marketFilter, marketProjections, marketSort, maxResults);

                result.AddRange(marketCatalogues);
            }

            //var marketCatalogues = _exchangeClient!.ListMarketCatalogue(
               // marketFilter, marketProjections, marketSort, maxResults);

            return result;
        }

        public IList<MarketBook> ListMarketBooks(IList<string> marketIds)
        {
            //as an example we requested runner metadata 
            PriceProjection priceProjection = new PriceProjection()
            {
                PriceData = new HashSet<PriceData>
                {
                    PriceData.EX_BEST_OFFERS,
                    PriceData.EX_TRADED
                },
            };

            var marketBooks = _exchangeClient!.ListMarketBook(
                marketIds, priceProjection);

            return marketBooks;
        }
    }
}


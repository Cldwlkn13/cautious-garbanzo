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
        private readonly IAuthClient _authClient;
        private readonly IOptions<ExchangeSettings> _options;
        private readonly IOptions<LoginSettings> _logins;

        private IExchangeClient? _exchangeClient;

        public ExchangeHandler(IAuthClient authClient, IOptions<ExchangeSettings> options, IOptions<LoginSettings> logins)
        {
            _authClient = authClient;
            _options = options;
            _logins = logins;

            SessionToken = string.Empty;

            Username = Environment.GetEnvironmentVariable("USERNAME") != null ?
                Environment.GetEnvironmentVariable("USERNAME")! :
                logins.Value.USERNAME!;

            Password = Environment.GetEnvironmentVariable("PASSWORD") != null ?
                Environment.GetEnvironmentVariable("PASSWORD")! :
                logins.Value.PASSWORD!;

            AppKey = Environment.GetEnvironmentVariable("APP_KEY") != null ?
                Environment.GetEnvironmentVariable("APP_KEY")! :
                logins.Value.APP_KEY!;

            if (!SessionValid())
            {
                Login();

                _exchangeClient = new ExchangeClient(
                    _options.Value.Url, AppKey, SessionToken);
            }
        }

        public string SessionToken { get; private set; }
        public DateTime TokenExpiry { get; set; }
        public string AppKey { get; private set; }

        private string Username { get; set; }
        private string Password { get; set; }

        public bool Login(string username = "", string password = "")
        {
            var loginResult = _authClient.Login(Username, Password) ??
                throw new NullReferenceException($"Login Failed");

            SessionToken = loginResult.Token;
            TokenExpiry = DateTime.UtcNow.AddHours(1);

            Console.WriteLine($"SESSION_TOKEN_RENEWED; " +
                $"ValidTo={TokenExpiry.ToString("dd-MM-yyyy HH:mm")}");

            return !string.IsNullOrEmpty(SessionToken);
        }

        public bool SessionValid()
        {
            return !string.IsNullOrEmpty(SessionToken) &&
                DateTime.UtcNow < TokenExpiry;
        }

        public IList<EventTypeResult> ListEventTypes()
        {
            var marketFilter = new MarketFilter();

            var eventTypes = _exchangeClient?.ListEventTypes(marketFilter) ??
                throw new NullReferenceException($"Event Types null.");

            return eventTypes;
        }

        public IList<EventResult> ListEvents(string eventTypeId = "7", TimeRange? timeRange = null)
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

        public IList<MarketCatalogue> ListMarketCatalogues(string eventTypeId = "7", TimeRange? timeRange = null, IEnumerable<string>? eventIds = null)
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

            marketFilter.MarketStartTime = time;
            marketFilter.MarketCountries = eventTypeId.CountryCodes();
            marketFilter.MarketTypeCodes = eventTypeId.MarketTypes();
            var marketSort = MarketSort.FIRST_TO_START;
            var maxResults = "1000";

            //as an example we requested runner metadata 
            ISet<MarketProjection> marketProjections = new HashSet<MarketProjection>();
            marketProjections.Add(MarketProjection.EVENT);
            marketProjections.Add(MarketProjection.MARKET_DESCRIPTION);

            var marketCatalogues = _exchangeClient!.ListMarketCatalogue(
                marketFilter, marketProjections, marketSort, maxResults);

            return marketCatalogues;
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


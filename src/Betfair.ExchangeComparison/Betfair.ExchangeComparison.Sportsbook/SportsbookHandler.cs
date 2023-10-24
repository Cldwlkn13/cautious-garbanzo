using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Exchange.Settings;
using Betfair.ExchangeComparison.Sportsbook.Clients;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Betfair.ExchangeComparison.Sportsbook.Settings;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Sportsbook;

public class SportsbookHandler : ISportsbookHandler
{
    private readonly IAuthClient _authClient;
    private readonly IOptions<SportsbookSettings> _options;
    private readonly IOptions<LoginSettings> _logins;
    private ISportsbookClient? _client;

    public SportsbookHandler(IAuthClient authClient, IOptions<SportsbookSettings> options, IOptions<LoginSettings> logins)
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

            var url = _options.Value.UseBetfair ?
                _options.Value.UrlBetfair :
                _options.Value.UrlPaddyPower;

            _client = new SportsbookClient(url, AppKey, SessionToken);
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
        TokenExpiry = DateTime.UtcNow.AddHours(6);

        Console.WriteLine($"SESSION_TOKEN_RENEWED; " +
            $"ValidTo={TokenExpiry.ToString("dd-MM-yyyy HH:mm")}");

        return SessionValid();
    }

    public bool SessionValid()
    {
        return !string.IsNullOrEmpty(SessionToken) &&
            DateTime.UtcNow < TokenExpiry;
    }

    public IEnumerable<EventTypeResult> ListEventTypes()
    {
        var marketFilter = new MarketFilter();

        var eventTypes = _client?.ListEventTypes(marketFilter) ??
            throw new NullReferenceException($"Event Types null.");

        return eventTypes;
    }

    public IEnumerable<CompetitionResult> ListCompetitions(string eventTypeId = "7", TimeRange? timeRange = null)
    {
        var time = new TimeRange();

        if (timeRange == null)
        {
            switch (eventTypeId)
            {
                case "7":
                    time = new TimeRange()
                    {
                        From = DateTime.Today,
                        To = DateTime.Today.AddDays(_options.Value.RacingQueryToDays)
                    };
                    break;
                case "1":
                    time = new TimeRange()
                    {
                        From = DateTime.Now,
                        To = DateTime.Now.AddHours(_options.Value.FootballQueryToHours)
                    };
                    break;
            }
        }
        else
        {
            time = timeRange;
        }

        var competitions = _client?.ListCompetitions(eventTypeId, time) ??
            throw new NullReferenceException($"Competitions null.");

        return competitions;
    }

    public IEnumerable<Event> ListEventsByEventType(string eventTypeId = "7", TimeRange? timeRange = null)
    {
        var time = new TimeRange();

        if (timeRange == null)
        {
            switch (eventTypeId)
            {
                case "7":
                    time = new TimeRange()
                    {
                        From = DateTime.Today,
                        To = DateTime.Today.AddDays(_options.Value.RacingQueryToDays)
                    };
                    break;
                case "1":
                    time = new TimeRange()
                    {
                        From = DateTime.Now,
                        To = DateTime.Now.AddHours(_options.Value.FootballQueryToHours)
                    };
                    break;
            }
        }
        else
        {
            time = timeRange;
        }

        var events = _client?.ListEventsByEventType(eventTypeId, time) ??
            throw new NullReferenceException($"Events null.");

        return events.Select(e => e.Event);
    }

    public Dictionary<Competition, List<Event>> ListEventsByCompetition(string eventTypeId, IEnumerable<Competition> competitions, TimeRange? timeRange = null)
    {
        var result = new Dictionary<Competition, List<Event>>();

        var time = new TimeRange();
        switch (eventTypeId)
        {
            case "7":
                time = new TimeRange()
                {
                    From = DateTime.Today,
                    To = DateTime.Today.AddDays(_options.Value.RacingQueryToDays)
                };
                break;
            case "1":
                time = new TimeRange()
                {
                    From = DateTime.Now,
                    To = DateTime.Now.AddHours(_options.Value.FootballQueryToHours)
                };
                break;
        }

        foreach (var competition in competitions)
        {
            var eventsInCompetition = _client?.ListEventsByCompetition(competition, time).Select(e => e.Event) ??
                throw new NullReferenceException($"Events in Competition={competition.Name} null.");

            result.Add(competition, new List<Event>());

            foreach (var eventInCompetition in eventsInCompetition)
            {
                result[competition].Add(eventInCompetition);
            }
        }

        return result;
    }

    public IEnumerable<MarketTypeResult> ListMarketTypes()
    {
        var marketTypes = _client?.ListMarketTypes("7") ??
            throw new NullReferenceException($"Events null.");

        return marketTypes;
    }

    public IEnumerable<MarketCatalogue> ListMarketCatalogues(ISet<string> eventIds, string eventTypeId = "7")
    {
        var marketTypes = new List<string>();

        switch (eventTypeId)
        {
            case "7":
                marketTypes = new List<string>() { "WIN" };
                break;
            case "1":
                marketTypes = new List<string>()
                {
                    "MATCH_ODDS",
                    "OVER_UNDER_15",
                    "OVER_UNDER_25",
                    "OVER_UNDER_35",
                    "BOTH_TEAMS_TO_SCORE"
                };
                break;
        }

        var marketCatalogues = _client?.ListMarketCatalogue(new SportsbookMarketFilter()
        {
            EventIds = eventIds,
            MarketTypes = marketTypes.ToArray()
        },
        maxResults: "100")
            ??
            throw new NullReferenceException($"Market Catalogues null.");

        return marketCatalogues;
    }

    public MarketDetails ListPrices(IEnumerable<string> marketIds)
    {
        var prices = _client?.ListMarketPrices(marketIds) ??
            throw new NullReferenceException($"Prices null.");

        return prices;
    }
}


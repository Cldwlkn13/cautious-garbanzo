using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Exchange.Settings;
using Betfair.ExchangeComparison.Sportsbook.Clients;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Sportsbook;

public class SportsbookHandler : ISportsbookHandler
{
    private readonly IAuthClient _authClient;
    private readonly IOptions<ExchangeSettings> _options;

    private ISportsbookClient? _client;

    public SportsbookHandler(IAuthClient authClient, IOptions<ExchangeSettings> options)
    {
        _authClient = authClient;
        _options = options;

        SessionToken = string.Empty;
        AppKey = _options.Value.AppKey;
    }

    public string SessionToken { get; private set; }
    public string AppKey { get; private set; }

    public bool Login(string username, string password)
    {
        var loginResult = _authClient.Login("", "") ??
            throw new NullReferenceException($"Login Failed");

        SessionToken = loginResult.Token;

        _client = new SportsbookClient(
            _options.Value.SportsbookUrl, AppKey, SessionToken);

        return SessionValid();
    }

    public bool SessionValid()
    {
        return !string.IsNullOrEmpty(SessionToken);
    }

    public IList<EventTypeResult> ListEventTypes()
    {
        var marketFilter = new MarketFilter();

        var eventTypes = _client?.ListEventTypes(marketFilter) ??
            throw new NullReferenceException($"Event Types null.");

        return eventTypes;
    }

    public IList<CompetitionResult> ListCompetitions(string eventTypeId = "7")
    {
        var competitions = _client?.ListCompetitions(eventTypeId, DateTime.Today, DateTime.Today.AddDays(1)) ??
            throw new NullReferenceException($"Competitions null.");

        return competitions;
    }

    public IList<EventResult> ListEventsByEventType(string eventTypeId = "7")
    {
        var time = new TimeRange();
        switch (eventTypeId)
        {
            case "7":
                time = new TimeRange()
                {
                    From = DateTime.Today,
                    To = DateTime.Today.AddDays(1)
                };
                break;
            case "1":
                time = new TimeRange()
                {
                    From = DateTime.Now,
                    To = DateTime.Now.AddHours(6)
                };
                break;
        }

        var events = _client?.ListEventsByEventType(eventTypeId, time) ??
            throw new NullReferenceException($"Events null.");

        return events;
    }

    public IList<MarketTypeResult> ListMarketTypes()
    {
        var marketTypes = _client?.ListMarketTypes("7") ??
            throw new NullReferenceException($"Events null.");

        return marketTypes;
    }

    public IList<MarketCatalogue> ListMarketCatalogues(ISet<string> eventIds, string eventTypeId = "7")
    {
        var marketTypes = new List<string>();

        switch (eventTypeId)
        {
            case "7":
                marketTypes = new List<string>() { "WIN" };
                break;
            case "1":
                marketTypes = new List<string>() { "MATCH_ODDS", "OVER_UNDER_15", "OVER_UNDER_25", "OVER_UNDER_35", "BOTH_TEAMS_TO_SCORE" };
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

    public MarketDetails ListPrices(IList<string> marketIds)
    {
        var prices = _client?.ListMarketPrices(marketIds) ??
            throw new NullReferenceException($"Prices null.");

        return prices;
    }

    public IList<CompetitionResult> ListCompetitions()
    {
        throw new NotImplementedException();
    }
}


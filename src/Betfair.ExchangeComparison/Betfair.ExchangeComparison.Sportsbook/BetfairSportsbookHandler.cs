using Betfair.ExchangeComparison.Auth.Interfaces;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Clients;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Betfair.ExchangeComparison.Sportsbook.Settings;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Sportsbook;

public class BetfairSportsbookHandler : IBetfairSportsbookHandler
{
    private readonly IOptions<SportsbookSettings> _options;
    private readonly IAuthHandler _authHandler;
    private ISportsbookClient? _client;

    private const Bookmaker _bookmaker = Bookmaker.BetfairSportsbook;

    public BetfairSportsbookHandler(IOptions<SportsbookSettings> options, IAuthHandler authHandler)
    {
        _authHandler = authHandler;
        _options = options;

        if (TryLogin())
        {
            _client = new SportsbookClient(
                _options.Value.UrlBetfair,
                _authHandler.AppKey,
                _authHandler.SessionTokens[_bookmaker]);
        }
    }

    public bool TryLogin()
    {
        var isLoginSuccessful = _authHandler.TryLogin(_bookmaker);
        _client = new SportsbookClient(
            _options.Value.UrlBetfair,
            _authHandler.AppKey,
            _authHandler.SessionTokens[_bookmaker]);
        return isLoginSuccessful;
    }

    public bool Login(string username = "", string password = "")
    {
        var isLoginSuccessful = _authHandler.Login(username, password, _bookmaker);
        _client = new SportsbookClient(
           _options.Value.UrlBetfair,
            _authHandler.AppKey,
            _authHandler.SessionTokens[_bookmaker]);
        return isLoginSuccessful;
    }

    public bool SessionValid() =>
        _authHandler.SessionValid(_bookmaker);

    public IEnumerable<EventTypeResult> ListEventTypes()
    {
        var marketFilter = new MarketFilter();

        try
        {
            var eventTypes = _client?
                .ListEventTypes(marketFilter) ??
                throw new NullReferenceException($"Event Types null.");

            //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
            //    $"Action={nameof(ListEventTypes)} " +
            //    $"Returning {eventTypes.Count()} Event Types!");

            return eventTypes;
        }
        catch (System.Exception exception)
        {
            Console.WriteLine($"{nameof(ListEventTypes)} {exception}");
            return Enumerable.Empty<EventTypeResult>();
        }
    }

    public IEnumerable<CompetitionResult> ListCompetitions(
        string eventTypeId = "7", TimeRange? timeRange = null)
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

        try
        {
            var competitions = _client?
                .ListCompetitions(eventTypeId, time) ??
                throw new NullReferenceException($"Competitions null.");

            //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
            //    $"Action={nameof(ListCompetitions)} " +
            //    $"Returning {competitions.Count()} Competitions!");

            return competitions;
        }
        catch(System.Exception exception)
        {
            Console.WriteLine($"{nameof(ListCompetitions)} {exception}");
            return new CompetitionResult[0];
        }
    }

    public IEnumerable<Event> ListEventsByEventType(
        string eventTypeId = "7", TimeRange? timeRange = null)
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

        try
        {
            var events = _client?
                .ListEventsByEventType(eventTypeId, time) ??
                throw new NullReferenceException($"Events null.");

            var response = events?.Select(e => e.Event) ?? new List<Event>();

            //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
            //    $"Action={nameof(ListEventsByEventType)} " +
            //    $"Returning {response.Count()} Events!");

            return response;
        }
        catch (System.Exception exception)
        {
            Console.WriteLine($"{nameof(ListEventsByEventType)} {exception}");
            return Enumerable.Empty<Event>();
        }
    }

    public Dictionary<Competition, List<Event>> ListEventsByCompetition(
        string eventTypeId, IEnumerable<Competition> competitions, TimeRange? timeRange = null)
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
            IEnumerable<Event> eventsInCompetition = new List<Event>();

            try
            {
                eventsInCompetition = _client?
                    .ListEventsByCompetition(competition, time)
                    .Select(e => e.Event) ??
                    throw new NullReferenceException($"Events in Competition={competition.Name} null.");
            }
            catch (System.Exception exception)
            {
                Console.WriteLine($"{nameof(ListEventsByCompetition)} {exception}");
            }

            if (!eventsInCompetition.Any()) continue;

            result.Add(competition, new List<Event>());

            foreach (var eventInCompetition in eventsInCompetition)
            {
                result[competition].Add(eventInCompetition);
            }
        }

        //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
        //    $"Action={nameof(ListEventsByCompetition)} " +
        //    $"Returning {result.Count()} Competitions With Events!");

        return result;
    }

    public IEnumerable<MarketTypeResult> ListMarketTypes()
    {
        try
        {
            var marketTypes = _client?
                 .ListMarketTypes("7") ??
                    throw new NullReferenceException($"Market Types null.");

            //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
            //    $"Action={nameof(ListMarketTypes)} " +
            //    $"Returning {marketTypes.Count()} Market Types!");

            return marketTypes;
        }
        catch (System.Exception exception)
        {
            Console.WriteLine($"{nameof(ListMarketTypes)} {exception}");
            return Enumerable.Empty<MarketTypeResult>();
        }
    }

    public IEnumerable<MarketCatalogue> ListMarketCatalogues(
        ISet<string> eventIds, string eventTypeId = "7")
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

        try
        {
            var marketCatalogues = _client?
                .ListMarketCatalogue(new SportsbookMarketFilter()
                {
                    EventIds = eventIds,
                    MarketTypes = marketTypes.ToArray()
                },
            maxResults: "100")
                ??
                throw new NullReferenceException($"Market Catalogues null.");

            //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
            //    $"Action={nameof(ListMarketCatalogues)} " +
            //    $"Returning {marketCatalogues.Count()} Market Catalogues!");

            return marketCatalogues;
        }
        catch (System.Exception exception)
        {
            Console.WriteLine($"{nameof(ListMarketCatalogues)} {exception}");
            return Enumerable.Empty<MarketCatalogue>();
        }
    }

    public MarketDetails ListPrices(IEnumerable<string> marketIds)
    {
        try
        {
            var prices = _client?
                .ListMarketPrices(marketIds) ??
                throw new NullReferenceException($"Prices null.");

            //Console.WriteLine($"Source={nameof(BetfairSportsbookHandler)} " +
            //    $"Action={nameof(ListPrices)} " +
            //    $"Returning {prices.marketDetails.Count()} Market Details!");

            return prices;
        }
        catch (System.Exception exception)
        {
            Console.WriteLine($"{nameof(ListPrices)} {exception}");
            return new MarketDetails();
        }
    }
}


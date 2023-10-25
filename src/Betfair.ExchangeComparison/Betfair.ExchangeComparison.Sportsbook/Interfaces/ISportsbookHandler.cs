using Betfair.ExchangeComparison.Domain.CommonInterfaces;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Sportsbook.Interfaces
{
    public interface ISportsbookHandler : IBetfairHandler
    {
        IEnumerable<EventTypeResult> ListEventTypes();
        IEnumerable<CompetitionResult> ListCompetitions(string eventTypeId = "7", TimeRange? timeRange = null);
        IEnumerable<Event> ListEventsByEventType(string eventTypeId = "7", TimeRange? timeRange = null);
        Dictionary<Competition, List<Event>> ListEventsByCompetition(string eventTypeId, IEnumerable<Competition> competitions, TimeRange? timeRange = null);
        IEnumerable<MarketTypeResult> ListMarketTypes();
        IEnumerable<MarketCatalogue> ListMarketCatalogues(ISet<string> eventIds, string eventTypeId = "7");
        MarketDetails ListPrices(IEnumerable<string> marketIds);
    }
}


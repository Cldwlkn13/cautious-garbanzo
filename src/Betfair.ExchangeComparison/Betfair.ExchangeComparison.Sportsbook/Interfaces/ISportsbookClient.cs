using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Sportsbook.Interfaces
{
    public interface ISportsbookClient
    {
        IEnumerable<EventTypeResult> ListEventTypes(MarketFilter marketFilter, string locale = null);
        IEnumerable<CompetitionResult> ListCompetitions(string eventTypeId, TimeRange timeRange, string locale = null);
        IEnumerable<EventResult> ListEventsByEventType(string eventTypeId, TimeRange timeRange);
        IEnumerable<EventResult> ListEventsByCompetition(Competition competition, TimeRange timeRange);
        IEnumerable<MarketTypeResult> ListMarketTypes(string eventTypeId);
        IEnumerable<MarketCatalogue> ListMarketCatalogue(SportsbookMarketFilter marketFilter, string maxResults = "1", string locale = null);
        MarketDetails ListMarketPrices(IEnumerable<string> marketIds);
    }
}


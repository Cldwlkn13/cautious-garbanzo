using System;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Sportsbook.Interfaces
{
    public interface ISportsbookClient
    {
        IList<EventTypeResult> ListEventTypes(MarketFilter marketFilter, string locale = null);

        IList<CompetitionResult> ListCompetitions(string eventTypeId, DateTime dateFrom, DateTime dateTo, string locale = null);

        IList<EventResult> ListEventsByEventType(string eventTypeId, TimeRange timeRange);

        IList<MarketTypeResult> ListMarketTypes(string eventTypeId);

        IList<MarketCatalogue> ListMarketCatalogue(SportsbookMarketFilter marketFilter, string maxResults = "1", string locale = null);

        MarketDetails ListMarketPrices(IList<string> marketIds);
    }
}


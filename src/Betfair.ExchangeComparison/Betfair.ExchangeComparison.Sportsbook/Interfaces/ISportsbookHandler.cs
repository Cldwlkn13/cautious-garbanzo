using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Sportsbook.Interfaces
{
    public interface ISportsbookHandler
    {
        bool Login(string username, string password);

        public string SessionToken { get; }

        public string AppKey { get; }

        bool SessionValid();

        IList<EventTypeResult> ListEventTypes();
        IList<CompetitionResult> ListCompetitions();
        IList<EventResult> ListEventsByEventType(string eventTypeId = "7");
        IList<MarketTypeResult> ListMarketTypes();
        IList<MarketCatalogue> ListMarketCatalogues(ISet<string> eventIds, string eventTypeId = "7");
        MarketDetails ListPrices(IList<string> marketIds);
    }
}


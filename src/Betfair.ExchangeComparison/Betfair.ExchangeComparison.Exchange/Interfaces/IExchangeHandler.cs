using System;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Exchange.Interfaces
{
    public interface IExchangeHandler
    {
        bool Login(string username = "", string password = "");

        public string SessionToken { get; }

        public string AppKey { get; }

        bool SessionValid();

        IList<EventTypeResult> ListEventTypes();
        IList<EventResult> ListEvents(string eventTypeId = "7", TimeRange? timeRange = null);
        IList<MarketCatalogue> ListMarketCatalogues(string eventTypeId = "7", TimeRange? timeRange = null, IEnumerable<string>? eventIds = null);
        IList<MarketBook> ListMarketBooks(IList<string> marketIds);
    }
}


using System;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Exchange.Interfaces
{
    public interface IExchangeHandler
    {
        bool Login(string username, string password);

        public string SessionToken { get; }

        public string AppKey { get; }

        bool SessionValid();

        IList<EventTypeResult> ListEventTypes();
        IList<MarketCatalogue> ListMarketCatalogues(string eventTypeId = "7");
        IList<MarketBook> ListMarketBooks(IList<string> marketIds);
    }
}


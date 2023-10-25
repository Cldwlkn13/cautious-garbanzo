using System;
using Betfair.ExchangeComparison.Domain.CommonInterfaces;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Exchange.Interfaces
{
    public interface IExchangeHandler : IBetfairHandler
    {
        IList<EventTypeResult> ListEventTypes();
        IList<CompetitionResult> ListCompetitions(string eventTypeId = "7", TimeRange? timeRange = null);
        IList<EventResult> ListEvents(string eventTypeId = "7", TimeRange? timeRange = null);
        Dictionary<Competition, IEnumerable<Event>> ListEventsByCompetition(IEnumerable<Competition> competitions, string eventTypeId = "7", TimeRange? timeRange = null);
        IList<MarketCatalogue> ListMarketCatalogues(string eventTypeId = "7", TimeRange? timeRange = null, IEnumerable<string>? eventIds = null);
        IList<MarketBook> ListMarketBooks(IList<string> marketIds);
    }
}


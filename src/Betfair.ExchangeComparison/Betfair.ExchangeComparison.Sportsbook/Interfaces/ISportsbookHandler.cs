﻿using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Sportsbook.Interfaces
{
    public interface ISportsbookHandler
    {
        bool Login(string username = "", string password = "");

        public string SessionToken { get; }

        public string AppKey { get; }

        bool SessionValid();

        IEnumerable<EventTypeResult> ListEventTypes();
        IEnumerable<CompetitionResult> ListCompetitions(string eventTypeId = "7", TimeRange? timeRange = null);
        IEnumerable<Event> ListEventsByEventType(string eventTypeId = "7", TimeRange? timeRange = null);
        Dictionary<Competition, List<Event>> ListEventsByCompetition(string eventTypeId, IEnumerable<Competition> competitions, TimeRange? timeRange = null);
        IEnumerable<MarketTypeResult> ListMarketTypes();
        IEnumerable<MarketCatalogue> ListMarketCatalogues(ISet<string> eventIds, string eventTypeId = "7");
        MarketDetails ListPrices(IEnumerable<string> marketIds);
    }
}


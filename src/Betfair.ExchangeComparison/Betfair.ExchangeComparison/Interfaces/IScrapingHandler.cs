using System;
using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingHandler
    {
        Task Handle(IEnumerable<CompoundEventWithMarketDetail> catalog);
        List<ScrapedEvent> GetScrapedEvents(Bookmaker bookmaker, DateTime dateTime);

        public ConcurrentDictionary<Bookmaker, Dictionary<DateTime, Dictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; }
    }
}


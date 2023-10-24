using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingOrchestrator
    {
        Task Orchestrate(IEnumerable<MarketDetailWithEvent> catalog, Bookmaker bookmaker);
        bool TryGetScrapedEvents(Bookmaker bookmaker, DateTime dateTime, out List<ScrapedEvent> result);
        Task<UsageModel> Usage();

        public ConcurrentDictionary<Bookmaker, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; }
    }
}


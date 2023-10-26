using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingOrchestrator
    {
        Task Orchestrate(IEnumerable<MarketDetailWithEvent> catalog, Provider provider);
        bool TryGetScrapedEvents(Provider provider, DateTime dateTime, out List<ScrapedEvent> result);
        Task<UsageModel> Usage();
        //public Dictionary<Provider, bool> SwitchBoard { get; }

        public ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; }
    }
}


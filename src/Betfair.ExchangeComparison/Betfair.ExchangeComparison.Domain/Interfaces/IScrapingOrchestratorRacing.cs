using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using System.Collections.Concurrent;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingOrchestratorRacing
    {
        Task Orchestrate(IEnumerable<MarketDetailWithEwc> catalog, Provider provider);

        bool TryGetScrapedEvents(Provider provider, DateTime dateTime, out List<ScrapedEvent> result);
        Task<UsageModel> Usage();

        public ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; }
    }
}

using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingOrchestrator<T>
    {
        Task Orchestrate(IEnumerable<MarketDetailWithEvent> catalog, Provider provider);
        Task Orchestrate(ExchangeCatalogue catalog, Provider provider);
        Task Orchestrate(Dictionary<EventWithCompetition, List<MarketDetail>> catalog, Provider provider);

        //Task OrchestrateSingleEvents(IEnumerable<MarketDetailWithEvent> catalog, Provider provider);

        //Task OrchestrateEnumerable(IEnumerable<MarketDetailWithEvent> catalog, Provider provider);
        //Task OrchestrateEnumerable(ExchangeCatalogue catalog, Provider provider);

        bool TryGetScrapedEvents(Provider provider, DateTime dateTime, out List<ScrapedEvent> result);
        Task<UsageModel> Usage();

        public ConcurrentDictionary<Provider, ConcurrentDictionary<DateTime, ConcurrentDictionary<string, ScrapedEvent>>> ScrapedEventsCatalog { get; }
    }
}


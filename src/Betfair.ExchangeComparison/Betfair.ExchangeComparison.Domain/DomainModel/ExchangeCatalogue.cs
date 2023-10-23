using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class ExchangeCatalogue
    {
        public ExchangeCatalogue()
        {
        }

        public IDictionary<string, Event> EventDictionary { get; set; }
        public IEnumerable<MarketCatalogue> MarketCatalogues { get; set; }
        public ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> MarketBooks { get; set; }
    }
}


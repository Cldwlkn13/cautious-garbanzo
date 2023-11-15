using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class ExchangeCatalogue
    {
        public ExchangeCatalogue()
        {
            EventDictionary = new Dictionary<string, Event>();
            MarketCatalogues = new List<MarketCatalogue>();
            MarketBooks = new ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>>();
        }

        public IDictionary<string, Event> EventDictionary { get; set; }
        public IEnumerable<MarketCatalogue> MarketCatalogues { get; set; }
        public ConcurrentDictionary<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> MarketBooks { get; set; }
    }
}


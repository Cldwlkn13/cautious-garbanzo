using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class EventWithMarketBooks
    {
        public Event Event { get; set; }
        public IDictionary<DateTime, IList<MarketBook>> MarketBooks { get; set;}
    }
}

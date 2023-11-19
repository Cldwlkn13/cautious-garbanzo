using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class MarketBooksByDateTime
    {
        public DateTime DateTime { get; set; }
        public IList<MarketBook> MarketBooks { get; set; }

        public MarketBooksByDateTime()
        {
            MarketBooks = new List<MarketBook>();
        }

        public MarketBooksByDateTime(KeyValuePair<DateTime, IList<MarketBook>> src) 
        { 
            DateTime = src.Key;
            MarketBooks = src.Value;
        }
    }
}

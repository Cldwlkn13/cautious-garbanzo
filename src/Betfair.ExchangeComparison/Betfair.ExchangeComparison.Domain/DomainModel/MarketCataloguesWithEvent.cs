using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class MarketCataloguesWithEvent
    {
        public MarketCataloguesWithEvent()
        {
            ExchangeMarketCatalogues = new List<MarketCatalogue>();
        }

        public EventWithCompetition EventWithCompetition { get; set; }
        public IEnumerable<MarketCatalogue> ExchangeMarketCatalogues { get; set; }
    }
}


using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class MarketCataloguesWithEwc
    {
        public MarketCataloguesWithEwc()
        {
            ExchangeMarketCatalogues = new List<MarketCatalogue>();
        }

        public EventWithCompetition EventWithCompetition { get; set; }
        public IEnumerable<MarketCatalogue> ExchangeMarketCatalogues { get; set; }
    }
}


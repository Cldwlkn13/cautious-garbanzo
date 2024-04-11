using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Pages.Model
{
    public class BaseCatalogModel
    {
        public SportsbookCatalogue SportsbookCatalogue { get; set; }
        public ExchangeCatalogue ExchangeCatalogue { get; set; }
        public List<MatchbookEvent> MatchbookCatalogue { get; set; }
        public bool HasEachWay { get; set; }
    }
}


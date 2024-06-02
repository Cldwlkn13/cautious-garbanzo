using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface ITradingHandler
    {
        Task<CatalogViewModel> TradeCatalogue(CatalogViewModel cvm);
    }
}

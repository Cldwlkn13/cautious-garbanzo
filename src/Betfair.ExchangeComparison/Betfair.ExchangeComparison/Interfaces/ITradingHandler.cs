using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface ITradingHandler
    {
        Task TradeCatalogue(CatalogViewModel cvm);
    }
}

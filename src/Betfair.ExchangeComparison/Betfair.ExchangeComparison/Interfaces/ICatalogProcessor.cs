using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Pages.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface ICatalogProcessor
    {
        Task<BaseCatalogModel?> Process(Sport sport);
    }
}

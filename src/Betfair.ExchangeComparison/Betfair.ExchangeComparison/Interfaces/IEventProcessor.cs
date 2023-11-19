using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IEventProcessor
    {
        Task<CatalogViewModel> Process(BaseCatalogModel baseCatalogModel, BasePageModel basePageModel, List<ScrapedEvent> scrapedEvents);
    }
}

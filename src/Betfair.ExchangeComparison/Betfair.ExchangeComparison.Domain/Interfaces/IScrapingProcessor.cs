using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Pages.Model;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingProcessor<T>
    {
        void ProcessStartStops(BasePageModel baseModel);
        bool TryProcessScrapedEvents(BasePageModel basePageModel, out List<ScrapedEvent> result);
    }
}

using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class EventProcessor
    {
        private readonly IMappingService _mappingService;

        public EventProcessor(IMappingService mappingService)
        {
            _mappingService = mappingService;

        }

        public async Task Process(BaseCatalogModel baseCatalogModel)
        {
            foreach (var sportsbookEvent in baseCatalogModel.SportsbookCatalogue.EventsWithMarketCatalogue.Keys)
            {
                var EventWithMarketBooks = _mappingService.MapEventToMarketBooks(
                            baseCatalogModel.ExchangeCatalogue.MarketBooks,
                            sportsbookEvent);

                if (!_mappingService.TryMapSportsbookMarketDetailsToEvent(
                    baseCatalogModel.SportsbookCatalogue.EventsWithMarketDetails,
                    sportsbookEvent, out IEnumerable<MarketDetail> MappedMarketDetailsForEvent))
                {
                    continue;
                }


            }
        }
    }
}


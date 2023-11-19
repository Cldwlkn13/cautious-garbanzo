using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMappingService _mappingService;
        private readonly IMarketProcessor _marketProcessor;
            
        public EventProcessor(IMappingService mappingService, IMarketProcessor marketProcessor)
        {
            _mappingService = mappingService;
            _marketProcessor = marketProcessor;
        }

        public async Task<CatalogViewModel> Process(BaseCatalogModel baseCatalogModel, BasePageModel basePageModel, List<ScrapedEvent> scrapedEvents)
        {
            var result = new CatalogViewModel();
           
            foreach (var ewc in baseCatalogModel.SportsbookCatalogue.EventsWithMarketCatalogue.Keys)
            {
                try
                {
                    //Map the Sportsbook Event (e.g. 15:00 Ascot) to the Exchange market books that refer to it
                    var EventWithMarketBooks = _mappingService.MapEventToMarketBooksObj(
                                baseCatalogModel.ExchangeCatalogue.MarketBooks,
                                ewc);

                    //Map the Sportsbook Market Details to that same Event
                    if (!_mappingService.TryMapSportsbookMarketDetailsToEvent(
                        baseCatalogModel.SportsbookCatalogue.EventsWithMarketDetails,
                        ewc, out IEnumerable<MarketDetail> MappedMarketDetailsForEvent))
                    {
                        //log problem here
                        continue;
                    }

                    //Get any scraped events, if they exist
                    var MappedScrapedEvent = MapScrapedEvent(basePageModel, scrapedEvents, ewc);

                    foreach (var MarketDetail in MappedMarketDetailsForEvent.Where(m => m.marketStatus == "OPEN"))
                    {
                        var mvm = await _marketProcessor.Process(basePageModel,
                             ewc, MarketDetail, EventWithMarketBooks, baseCatalogModel.HasEachWay, MappedScrapedEvent);

                        if (mvm == null) continue;

                        result.Markets.Add(mvm);
                        result.BestWinRunners.AddRange(mvm.BestRunners);
                        result.BestEachWayRunners.AddRange(mvm.BestEachWayRunners);
                    }
                }
                catch(Exception exception)
                {
                    Console.WriteLine($"EVENT_COMPARISON_EXCEPTION; " +
                        $"Exception={exception.Message}; " +
                        $"Event={ewc.Event.Name}");

                    continue;
                }
            }

            return result;
        }

        private ScrapedEvent MapScrapedEvent(BasePageModel basePageModel, List<ScrapedEvent> scrapedEvents, EventWithCompetition ewc)
        {
            var mappedScrapedEvent = new ScrapedEvent();
            if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
            {
                if (!_mappingService.TryMapScrapedEvent(scrapedEvents, ewc, out mappedScrapedEvent))
                {
                    Console.WriteLine($"SCRAPED_EVENT_MAPPING_FAIL; " +
                        $"Event={ewc.Event.Name}");

                    return new ScrapedEvent();
                }
            }
            return mappedScrapedEvent;
        }
    }
}


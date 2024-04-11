using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Matchbook;
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

        public async Task<CatalogViewModel> Process(BaseCatalogModel baseCatalogModel, BasePageModel basePageModel, 
            List<ScrapedEvent> scrapedEvents)
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
                    var MappedScrapedEvent = new ScrapedEvent();
                    if (basePageModel.Sport == Sport.Football) // we can map football by the event name
                    {
                        MappedScrapedEvent = MapScrapedEvent(basePageModel, scrapedEvents, ewc);
                    }

                    _ = _mappingService.TryMapMatchbookEvents(baseCatalogModel.MatchbookCatalogue, ewc, 
                        out List<MatchbookEvent> matchbookEventsAtVenue);

                    foreach (var MarketDetail in MappedMarketDetailsForEvent.Where(m => m.marketStatus == "OPEN"))
                    {
                        if (basePageModel.Sport == Sport.Racing) //we have to map racing using the race time on the market detail
                        {
                            MappedScrapedEvent = MapScrapedEvent(basePageModel, scrapedEvents, ewc, MarketDetail);
                        }

                        _ = _mappingService.TryMapMatchbookEventToMarketDetail(matchbookEventsAtVenue, MarketDetail,
                            out MatchbookEvent mappedMatchbookEvent);

                        var mvm = await _marketProcessor.Process(basePageModel,
                             ewc, MarketDetail, EventWithMarketBooks, baseCatalogModel.HasEachWay, MappedScrapedEvent, mappedMatchbookEvent);

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

        private ScrapedEvent? MapScrapedEvent(BasePageModel basePageModel, List<ScrapedEvent> scrapedEvents, EventWithCompetition ewc, MarketDetail? marketDetail = null)
        {
            var mappedScrapedEvent = new ScrapedEvent();
            if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
            {
                switch(basePageModel.Sport)
                {
                    case Sport.Football:
                        if (!_mappingService.TryMapScrapedEvent(scrapedEvents, ewc, out mappedScrapedEvent))
                        {
                            Console.WriteLine($"SCRAPED_EVENT_MAPPING_FAIL; " +
                                $"Event={ewc.Event.Name}");

                            return null;
                        }
                        break;

                    case Sport.Racing:
                        if (!_mappingService.TryMapScrapedEvent(scrapedEvents, ewc, marketDetail, out mappedScrapedEvent))
                        {
                            Console.WriteLine($"SCRAPED_EVENT_MAPPING_FAIL; " +
                                $"Event={ewc.Event.Name}");

                            return null;
                        }
                        break;
                }
                

            }
            return mappedScrapedEvent;
        }
    }
}


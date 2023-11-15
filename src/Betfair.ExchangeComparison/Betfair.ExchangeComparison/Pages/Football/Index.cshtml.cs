using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Processors;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Betfair.ExchangeComparison.Pages.Football
{
    public class IndexModel : PageModel
    {
        private readonly CatalogProcessor _catalogProcessor;
        private readonly ScrapingProcessor<SportFootball> _scrapingProcessor;

        //mapping and comparison
        private readonly IMappingService _mappingService;
        private readonly IPricingComparisonHandler _pricingComparisonHandler;

        [BindProperty]
        public RacingFormModel FormModel { get; set; }
        public List<SelectListItem> SelectListBookmakers { get; set; }
        public CatalogViewModel CatalogViewModel { get; set; }

        public IndexModel(IMappingService mappingService, IPricingComparisonHandler pricingComparisonHandler, CatalogProcessor catalogProcessor, ScrapingProcessor<SportFootball> scrapingProcessor)
        {
            _catalogProcessor = catalogProcessor;
            _mappingService = mappingService;
            _pricingComparisonHandler = pricingComparisonHandler;
            _scrapingProcessor = scrapingProcessor;

            CatalogViewModel = new CatalogViewModel();
        }

        public async Task<PageResult> OnGet()
        {
            var bestWinRunners = new List<BestRunner>();
            var markets = new List<MarketViewModel>();

            var basePageModel = PageProcessor.Process(HttpContext.Session, Sport.Football);
            SelectListBookmakers = basePageModel.SelectListBookmakers;

            _scrapingProcessor.ProcessStartStops(basePageModel);

            try
            {
                var baseCatalogModel = await _catalogProcessor.Process();

                if (!_scrapingProcessor.TryProcessScrapedEvents(
                    basePageModel, out List<ScrapedEvent> scrapedEvents))
                {
                    return Page();
                }

                foreach (var @event in baseCatalogModel.SportsbookCatalogue.EventsWithMarketCatalogue.Keys)
                {
                    try
                    {
                        KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> eventWithMarketBooks = _mappingService.MapEventToMarketBooks(
                            baseCatalogModel.ExchangeCatalogue.MarketBooks, @event);

                        if (!_mappingService.TryMapSportsbookMarketDetailsToEvent(
                            baseCatalogModel.SportsbookCatalogue.EventsWithMarketDetails, @event, out IEnumerable<MarketDetail> mappedMarketDetailsForEvent))
                        {
                            continue;
                        }

                        var mappedScrapedEvent = new ScrapedEvent();
                        if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
                        {
                            if (!_mappingService.TryMapScrapedEvent(scrapedEvents, @event, out mappedScrapedEvent))
                            {
                                Console.WriteLine($"SCRAPED_EVENT_MAPPING_FAIL; " +
                                    $"Event={@event.Event.Name}");

                                continue;
                            }
                        }

                        foreach (var marketDetail in mappedMarketDetailsForEvent.Where(m => m.marketStatus == "OPEN"))
                        {
                            try
                            {
                                if (!_mappingService.TryMapMarketsBooksToSportsbookMarketDetail(
                                    eventWithMarketBooks, marketDetail, out KeyValuePair<DateTime, IList<MarketBook>> eventMarketBooks))
                                {
                                    continue;
                                }

                                ScrapedMarket mappedScrapedMarket = new ScrapedMarket();
                                if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
                                {
                                    if (!_mappingService.TryMapScrapedMarket(mappedScrapedEvent, marketDetail, out mappedScrapedMarket))
                                    {
                                        Console.WriteLine($"SCRAPED_MARKET_MAPPING_FAIL; " +
                                            $"Event={@event.Event.Name}; " +
                                            $"Market={marketDetail.marketName} {marketDetail.marketStartTime}");
                                    }
                                }

                                if (!_mappingService.TryMapMarketBook(eventMarketBooks, marketDetail, out MarketBook mappedWinMarketBook))
                                {
                                    continue;
                                }

                                var winOverround = marketDetail.WinOverround();

                                var runners = new List<RunnerViewModel>();
                                foreach (var sportsbookRunner in marketDetail.runnerDetails)
                                {
                                    var scrapedRunnerIsValid = false;
                                    ScrapedRunner mappedScrapedRunner = new ScrapedRunner();

                                    try
                                    {
                                        if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
                                        {
                                            if (_mappingService.TryMapScrapedRunner(mappedScrapedMarket, sportsbookRunner, out mappedScrapedRunner))
                                            {
                                                if (mappedScrapedEvent.ScrapedAt > DateTime.UtcNow.AddSeconds(-90))
                                                {
                                                    scrapedRunnerIsValid = true;
                                                }
                                            }

                                        }
                                    }
                                    catch (System.Exception exception)
                                    {
                                        Console.WriteLine($"SCRAPE_MAPPING_EXCEPTION; " +
                                            $"Exception={exception.Message}; " +
                                            $"Runner={sportsbookRunner.selectionName}; " +
                                            $"Market={marketDetail.marketName} {marketDetail.marketStartTime}; " +
                                            $"Event={@event.Event.Name}");
                                    }

                                    try
                                    {
                                        if (sportsbookRunner.runnerStatus != "ACTIVE" ||
                                            sportsbookRunner.winRunnerOdds == null)
                                        {
                                            continue;
                                        }

                                        if (!_mappingService.TryMapRunner(mappedWinMarketBook, sportsbookRunner,
                                            out var mappedExchangeWinRunner))
                                        {
                                            continue;
                                        }

                                        var rpo = new RunnerPriceOverview();

                                        if (scrapedRunnerIsValid)
                                        {
                                            rpo = new RunnerPriceOverview(
                                                @event,
                                                marketDetail,
                                                mappedScrapedMarket,
                                                mappedExchangeWinRunner,
                                                sportsbookRunner,
                                                mappedScrapedRunner,
                                                null,
                                                basePageModel.Bookmaker,
                                                mappedScrapedEvent);
                                        }
                                        else if (!basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
                                        {
                                            rpo = new RunnerPriceOverview(
                                                @event,
                                                marketDetail,
                                                sportsbookRunner,
                                                mappedExchangeWinRunner);
                                        }


                                        bestWinRunners = _pricingComparisonHandler.TryAddToBestRunnersWinOnly(bestWinRunners, rpo);

                                        var rvm = new RunnerViewModel()
                                        {
                                            SportsbookRunner = sportsbookRunner,
                                            ExchangeWinRunner = mappedExchangeWinRunner,
                                            SportsbookWinPrice = sportsbookRunner.winRunnerOdds.@decimal,
                                            ExpectedExchangeWinPrice = rpo.ExpectedWinPrice,
                                            WinExpectedValue = rpo.ExpectedValueWin,
                                            WinnerOddsString = rpo.WinnerOddsString,
                                        };

                                        runners.Add(rvm);
                                    }
                                    catch (System.Exception exception)
                                    {
                                        Console.WriteLine($"MARKET_COMPARISON_EXCEPTION; " +
                                            $"Exception={exception.Message}; " +
                                            $"Market={marketDetail.marketName} {marketDetail.marketStartTime}; " +
                                            $"Event={@event.Event.Name}");
                                    }
                                }

                                var vm = new MarketViewModel(@event.Event);
                                vm.SportsbookMarket = marketDetail;
                                vm.Runners = runners.OrderBy(r => r.SportsbookRunner.winRunnerOdds.@decimal);
                                vm.WinOverround = winOverround;

                                markets.Add(vm);

                            }
                            catch (System.Exception exception)
                            {
                                Console.WriteLine($"MARKET_COMPARISON_EXCEPTION; " +
                                    $"Exception={exception.Message}; " +
                                    $"Market={marketDetail.marketName} {marketDetail.marketStartTime}; " +
                                    $"Event={@event.Event.Name}");
                            }
                        }
                    }
                    catch (System.Exception exception)
                    {
                        Console.WriteLine($"EVENT_COMPARISON_EXCEPTION; " +
                            $"Exception={exception.Message}; " +
                            $"Event={@event.Event.Name}");
                    }
                }

                CatalogViewModel.Markets = markets;
                CatalogViewModel.BestWinRunners = bestWinRunners;

                return Page();
            }
            catch (APINGException apiException)
            {
                Console.WriteLine($"APING_EXCEPTION; " +
                    $"Exception={apiException.Message};" +
                    $"ErrorCode={apiException.ErrorCode};");

                return Page();
            }
            catch (System.Exception exception)
            {
                Console.WriteLine($"CATALOG_BUILD_EXCEPTION; " +
                    $"Exception={exception.Message}");

                return Page();
            }
        }

        public async Task<IActionResult> OnPost(RacingFormModel formModel)
        {
            HttpContext.Session.SetString(
                "Bookmaker-Football",
                formModel.Bookmaker.ToString());

            return await OnGet();
        }
    }
}

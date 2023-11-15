using System.Collections.Concurrent;
using System.Text;
using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Extensions;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Betfair.ExchangeComparison.Pages.Racing
{
    public class IndexModel : PageModel
    {
        //services
        private readonly ICatalogService _catalogService;

        //mapping and comparison
        private readonly IPricingComparisonHandler _pricingComparisonHandler;
        private readonly IMappingService _mappingService;

        //scraping
        private readonly IScrapingOrchestrator<SportRacing> _scrapingOrchestrator;
        private readonly IScrapingControl<SportRacing> _scrapingControl;

        private Bookmaker[] isScrapableBookmaker = new Bookmaker[]
        {
            Bookmaker.Boylesports,
            Bookmaker.WilliamHill,
            Bookmaker.Ladbrokes
        };

        public CatalogViewModel CatalogViewModel { get; set; }

        [BindProperty]
        public RacingFormModel FormModel { get; set; }

        public List<SelectListItem> SelectListBookmakers { get; set; }

        public IndexModel(ICatalogService catalogService, IScrapingOrchestrator<SportRacing> scrapingOrchestrator, IPricingComparisonHandler pricingComparisonHandler, IMappingService mappingService, IScrapingControl<SportRacing> scrapingControl)
        {
            _catalogService = catalogService;
            _scrapingOrchestrator = scrapingOrchestrator;
            _pricingComparisonHandler = pricingComparisonHandler;
            _mappingService = mappingService;
            _scrapingControl = scrapingControl;

            CatalogViewModel = new CatalogViewModel();
            SelectListBookmakers = typeof(Bookmaker).SelectList(cases: new string[] {
                Bookmaker.BetfairSportsbook.ToString(),
                Bookmaker.Boylesports.ToString(),
                Bookmaker.WilliamHill.ToString(),
                Bookmaker.Ladbrokes.ToString(),
                Bookmaker.BoylesportsDirect.ToString(),
            });
        }

        public async Task<IActionResult> OnGet()
        {
            var bestWinRunners = new List<BestRunner>();
            var bestEachWayRunners = new List<BestRunner>();
            var marketViewModels = new List<MarketViewModel>();

            var bookmakerString = HttpContext.Session.GetString("Bookmaker-Racing") ?? "Other";
            var bookmaker = Bookmaker.Unknown;

            if (!Enum.TryParse(typeof(Bookmaker), bookmakerString, out var savedBookmaker))
            {
                bookmaker = Bookmaker.BetfairSportsbook;
            }
            else
            {
                bookmaker = (Bookmaker)Enum.Parse(typeof(Bookmaker), savedBookmaker.ToString());
            }

            SelectListBookmakers.FirstOrDefault(bm => bm.Text == bookmaker.ToString())!.Selected = true;

            var provider = MapProviderToBookmaker();

            Provider MapProviderToBookmaker()
            {
                switch (bookmaker)
                {
                    case Bookmaker.Boylesports:
                    case Bookmaker.Ladbrokes:
                    case Bookmaker.WilliamHill:
                        return Provider.Oddschecker;
                    case Bookmaker.BoylesportsDirect:
                        return Provider.BoylesportsDirect;
                    default:
                        return Provider.BetfairSportsbook;
                }
            }

            foreach (var prv in _scrapingControl.SwitchBoard.Keys
                .Where(p => p != provider))
            {
                if (_scrapingControl.SwitchBoard[prv])
                {
                    Console.WriteLine($"Stopping {prv} scraping!");
                    _scrapingControl.Stop(prv);
                }
            }

            if (isScrapableBookmaker.Contains(bookmaker) && !_scrapingControl.SwitchBoard[provider])
            {

                Console.WriteLine($"Starting {bookmaker} scraping!");
                _scrapingControl.Start(provider);
            }

            try
            {
                var t1 = _catalogService.GetSportsbookCatalogue(
                    Sport.Racing,
                    BetfairQueryExtensions.TimeRangeForNextDays(1), bookmaker);

                var t2 = _catalogService.GetExchangeCatalogue(
                    Sport.Racing,
                    BetfairQueryExtensions.TimeRangeForNextDays(1));

                await Task.WhenAll(new Task[] { t1, t2 });

                var sportsbookCatalogue = t1.Result;
                var exchangeCatalogue = t2.Result;

                List<ScrapedEvent> scrapedEvents = new List<ScrapedEvent>();

                if (isScrapableBookmaker.Contains(bookmaker))
                {
                    _scrapingOrchestrator.TryGetScrapedEvents(
                        provider, DateTime.Today, out scrapedEvents);
                }

                foreach (var @event in sportsbookCatalogue.EventsWithMarketCatalogue.Keys)
                {
                    try
                    {
                        KeyValuePair<Event, ConcurrentDictionary<DateTime, IList<MarketBook>>> eventWithMarketBooks = _mappingService.MapEventToMarketBooks(exchangeCatalogue.MarketBooks, @event);

                        if (!_mappingService.TryMapSportsbookMarketDetailsToEvent(
                            sportsbookCatalogue.EventsWithMarketDetails, @event, out IEnumerable<MarketDetail> mappedMarketDetailsForEvent))
                        {
                            continue;
                        }

                        foreach (var marketDetail in mappedMarketDetailsForEvent.Where(m => m.marketStatus == "OPEN"))
                        {

                            try
                            {
                                var mappedScrapedEvent = new ScrapedEvent();
                                if (isScrapableBookmaker.Contains(bookmaker))
                                {
                                    if (!_mappingService.TryMapScrapedEvent(scrapedEvents, @event, marketDetail, out mappedScrapedEvent))
                                    {
                                        Console.WriteLine($"SCRAPED_EVENT_MAPPING_FAIL; " +
                                            $"Event={@event.Event.Name}");
                                    }
                                }

                                if (!_mappingService.TryMapMarketsBooksToSportsbookMarketDetail(
                                    eventWithMarketBooks, marketDetail, out KeyValuePair<DateTime, IList<MarketBook>> eventMarketBooks))
                                {
                                    continue;
                                }

                                ScrapedMarket mappedScrapedMarket = new ScrapedMarket();
                                int numberOfPlaces = 0;
                                int eachWayFraction = 0;
                                if (isScrapableBookmaker.Contains(bookmaker))
                                {
                                    if (!_mappingService.TryMapScrapedMarket(mappedScrapedEvent, out mappedScrapedMarket))
                                    {
                                        Console.WriteLine($"SCRAPED_MARKET_MAPPING_FAIL; " +
                                            $"Event={@event.Event.Name}; " +
                                            $"Market={marketDetail.marketName} {marketDetail.marketStartTime}");

                                        numberOfPlaces = marketDetail.numberOfPlaces;
                                        eachWayFraction = marketDetail.placeFractionDenominator;

                                    }
                                    else
                                    {
                                        if (mappedScrapedMarket.ScrapedEachWayTerms != null && mappedScrapedMarket.ScrapedEachWayTerms.TryGetScrapedEachWayTermsByBookmaker(bookmaker, out var scrapedEachWayTerms))
                                        {
                                            numberOfPlaces = scrapedEachWayTerms.NumberOfPlaces;
                                            eachWayFraction = scrapedEachWayTerms.EachWayFraction;
                                        }

                                    }
                                }
                                else
                                {
                                    numberOfPlaces = marketDetail.numberOfPlaces;
                                    eachWayFraction = marketDetail.placeFractionDenominator;
                                }

                                if (!_mappingService.TryMapMarketBook(eventMarketBooks, 1, out MarketBook mappedWinMarketBook))
                                {
                                    continue;
                                }

                                if (!_mappingService.TryMapMarketBook(eventMarketBooks, numberOfPlaces, out MarketBook mappedPlaceMarketBook))
                                {
                                    continue;
                                }

                                var winOverround = marketDetail.WinOverround();
                                var placeOverround = marketDetail.PlaceOverround();

                                var runners = new List<RunnerViewModel>();
                                foreach (var sportsbookRunner in marketDetail.runnerDetails)
                                {
                                    var scrapedRunnerIsValid = false;
                                    ScrapedRunner mappedScrapedRunner = new ScrapedRunner();

                                    try
                                    {
                                        if (isScrapableBookmaker.Contains(bookmaker))
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
                                        if (sportsbookRunner.runnerOrder >= 98 ||
                                            sportsbookRunner.runnerStatus != "ACTIVE" ||
                                            sportsbookRunner.winRunnerOdds == null)
                                        {
                                            continue;
                                        }

                                        if (!_mappingService.TryMapRunner(mappedWinMarketBook, sportsbookRunner,
                                            out var mappedExchangeWinRunner))
                                        {
                                            continue;
                                        }

                                        if (!_mappingService.TryMapRunner(mappedPlaceMarketBook, sportsbookRunner,
                                            out var mappedExchangePlaceRunner))
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
                                                mappedExchangePlaceRunner,
                                                bookmaker);
                                        }
                                        else
                                        {
                                            rpo = new RunnerPriceOverview(
                                                @event,
                                                marketDetail,
                                                sportsbookRunner,
                                                mappedExchangeWinRunner,
                                                mappedExchangePlaceRunner);
                                        }

                                        bestWinRunners = _pricingComparisonHandler.TryAddToBestRunnersWinOnly(bestWinRunners, rpo);
                                        bestEachWayRunners = _pricingComparisonHandler.TryAddToBestRunnersEachWay(bestEachWayRunners, rpo);

                                        var rvm = new RunnerViewModel()
                                        {
                                            SportsbookRunner = sportsbookRunner,
                                            ExchangeWinRunner = mappedExchangeWinRunner,
                                            ExchangePlaceRunner = mappedExchangePlaceRunner,
                                            SportsbookWinPrice = sportsbookRunner.winRunnerOdds.@decimal,
                                            SportsbookPlacePrice = rpo.EachWayPlacePart,
                                            ExpectedExchangeWinPrice = rpo.ExpectedWinPrice,
                                            ExpectedExchangePlacePrice = rpo.ExpectedPlacePrice,
                                            EachWayExpectedValue = rpo.ExpectedValueEachWay,
                                            WinExpectedValue = rpo.ExpectedValueWin,
                                            PlaceExpectedValue = rpo.ExpectedValuePlace,
                                            WinnerOddsString = rpo.WinnerOddsString,
                                        };

                                        runners.Add(rvm);
                                    }
                                    catch (System.Exception exception)
                                    {
                                        Console.WriteLine($"RUNNER_PRICE_COMPARISON_EXCEPTION; " +
                                            $"Exception={exception.Message}; " +
                                            $"Runner={sportsbookRunner.selectionName}; " +
                                            $"Market={marketDetail.marketName} {marketDetail.marketStartTime}; " +
                                            $"Event={@event.Event.Name}");
                                    }
                                }

                                var vm = new MarketViewModel(@event.Event);
                                vm.SportsbookMarket = marketDetail;
                                vm.Runners = runners.OrderBy(r => r.SportsbookRunner.winRunnerOdds.@decimal);
                                vm.WinOverround = winOverround;
                                vm.EachWayPlaceOverround = placeOverround;

                                marketViewModels.Add(vm);
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

                var usageModel = await _scrapingOrchestrator.Usage();

                CatalogViewModel.Markets = marketViewModels;
                CatalogViewModel.BestWinRunners = bestWinRunners;
                CatalogViewModel.BestEachWayRunners = bestEachWayRunners;
                CatalogViewModel.UsageModel = usageModel;

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
            HttpContext.Session.SetString("Bookmaker-Racing", formModel.Bookmaker.ToString());

            return await OnGet();
        }
    }
}

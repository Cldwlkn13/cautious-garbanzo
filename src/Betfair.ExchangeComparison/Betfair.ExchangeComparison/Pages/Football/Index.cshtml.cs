using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages.Football
{
    public class IndexModel : PageModel
    {
        private readonly IExchangeHandler _exchangeHandler;
        private readonly IBetfairSportsbookHandler _bfSportsbookHandler;
        private readonly ICatalogService _catalogService;
        private readonly IMappingService _mappingService;
        private readonly IPricingComparisonHandler _pricingComparisonHandler;

        const string EventTypeId = "1";

        public CatalogViewModel CatalogViewModel { get; set; }

        public IndexModel(IExchangeHandler exchangeHandler, IBetfairSportsbookHandler bfSportsbookHandler, ICatalogService catalogService,
            IMappingService mappingService, IPricingComparisonHandler pricingComparisonHandler)
        {
            _exchangeHandler = exchangeHandler;
            _bfSportsbookHandler = bfSportsbookHandler;
            _catalogService = catalogService;
            _mappingService = mappingService;
            _pricingComparisonHandler = pricingComparisonHandler;

            CatalogViewModel = new CatalogViewModel();
        }

        public async Task<PageResult> OnGet()
        {
            var bestWinRunners = new List<BestRunner>();
            var markets = new List<MarketViewModel>();

            try
            {
                var t1 = _catalogService.GetSportsbookCatalogue(
                    Sport.Football,
                    BetfairQueryExtensions.TimeRangeForNextDays(1), Bookmaker.BetfairSportsbook);

                var t2 = _catalogService.GetExchangeCatalogue(
                    Sport.Football,
                    BetfairQueryExtensions.TimeRangeForNextDays(1));

                await Task.WhenAll(new Task[] { t1, t2 });

                var sportsbookCatalogue = t1.Result;
                var exchangeCatalogue = t2.Result;

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
                                if (!_mappingService.TryMapMarketsBooksToSportsbookMarketDetail(
                                    eventWithMarketBooks, marketDetail, out KeyValuePair<DateTime, IList<MarketBook>> eventMarketBooks))
                                {
                                    continue;
                                }

                                if (!_mappingService.TryMapMarketBook(eventMarketBooks, marketDetail, out MarketBook mappedWinMarketBook))
                                {
                                    continue;
                                }

                                var winOverround = marketDetail.WinOverround();

                                var runners = new List<RunnerViewModel>();
                                foreach (var sportsbookRunner in marketDetail.runnerDetails)
                                {
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

                                        var rpo = new RunnerPriceOverview(
                                                @event,
                                                marketDetail,
                                                sportsbookRunner,
                                                mappedExchangeWinRunner);


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
    }
}

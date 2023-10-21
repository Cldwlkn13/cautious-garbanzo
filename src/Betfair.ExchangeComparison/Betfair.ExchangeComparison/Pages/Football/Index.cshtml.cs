using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Services;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages.Football
{
    public class IndexModel : PageModel
    {
        private readonly IExchangeHandler _exchangeHandler;
        private readonly ISportsbookHandler _sportsbookHandler;
        private readonly ICatalogService _catalogService;

        const string EventTypeId = "1";

        public CatalogViewModel CatalogViewModel { get; set; }

        public IndexModel(IExchangeHandler exchangeHandler, ISportsbookHandler sportsbookHandler, ICatalogService catalogService)
        {
            _exchangeHandler = exchangeHandler;
            _sportsbookHandler = sportsbookHandler;
            _catalogService = catalogService;

            CatalogViewModel = new CatalogViewModel();
        }

        public PageResult OnGet()
        {
            var bestWinRunners = new List<BestRunner>();

            var eventDict = _catalogService.GetExchangeEventsWithMarkets(EventTypeId);
            var marketCatalogues = _catalogService.GetExchangeMarketCatalogues(EventTypeId, eventDict.Keys.ToList());
            var marketBooks = _catalogService.GetExchangeMarketBooks(marketCatalogues, eventDict);

            var sportsbookEventsWithMarkets = _catalogService.GetSportsbookEventsWithMarkets(EventTypeId);
            var sportsbookEventsWithPrices = _catalogService.GetSportsbookEventsWithPrices(sportsbookEventsWithMarkets);

            var markets = new List<MarketViewModel>();

            foreach (var @event in sportsbookEventsWithPrices.Keys)
            {
                try
                {
                    var exchangeEventWithMarketBooks = marketBooks.FirstOrDefault(e => e.Key.Id == @event.Id);

                    if (exchangeEventWithMarketBooks.Key == null)
                    {
                        continue;
                    }

                    var mappedEvent = sportsbookEventsWithPrices[@event];

                    if (mappedEvent == null)
                    {
                        continue;
                    }

                    foreach (var marketDetail in mappedEvent)
                    {
                        var exchangeMarketBooks = exchangeEventWithMarketBooks.Value
                            .FirstOrDefault(m => m.Key == marketDetail.marketStartTime);

                        if (exchangeMarketBooks.Value == null) continue;

                        var mappedMarketBook = exchangeMarketBooks.Value
                            .FirstOrDefault(m => m.MarketId == marketDetail.linkedMarketId);

                        if (mappedMarketBook == null) continue;

                        var winOverround = marketDetail.runnerDetails
                            .Where(r => r.winRunnerOdds != null && r.winRunnerOdds.@decimal > 0 && r.runnerStatus == "ACTIVE")
                            .Sum(r => 1 / r.winRunnerOdds.@decimal) * 100;

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

                                var mappedExchangeWinRunner = mappedMarketBook?.Runners
                                    .FirstOrDefault(r => r.SelectionId == sportsbookRunner.selectionId);

                                double? bestPinkWin = 0;
                                double? bestPinkWinSize = 0;
                                double? bestBlueWin = 0;

                                if (mappedExchangeWinRunner != null)
                                {
                                    if (mappedExchangeWinRunner.ExchangePrices != null)
                                    {
                                        if (mappedExchangeWinRunner.ExchangePrices.AvailableToLay.Any())
                                        {
                                            bestPinkWin = mappedExchangeWinRunner.ExchangePrices.AvailableToLay[0]?.Price;
                                            bestPinkWinSize = mappedExchangeWinRunner.ExchangePrices.AvailableToLay[0]?.Size;
                                        }

                                        if (mappedExchangeWinRunner.ExchangePrices.AvailableToBack.Any())
                                        {
                                            bestBlueWin = mappedExchangeWinRunner.ExchangePrices.AvailableToBack[0]?.Price;
                                        }
                                    }
                                }

                                var winnerOddsString = $"{sportsbookRunner.winRunnerOdds.numerator}/{sportsbookRunner.winRunnerOdds.denominator}";

                                var winSpread = bestPinkWin != null && bestPinkWin > 0 && bestBlueWin != null && bestBlueWin > 0 ? bestPinkWin - bestBlueWin : 0;
                                var expectedWinPrice = bestPinkWin != null && bestPinkWin > 0 && winSpread != null ? bestPinkWin - (winSpread * 0.03) : 1;

                                var expectedValueWin = ExpectedValue(sportsbookRunner.winRunnerOdds.@decimal, expectedWinPrice.Value);

                                if (expectedValueWin > -0.03 && expectedWinPrice > 1)
                                {
                                    bestWinRunners.Add(new BestRunner()
                                    {
                                        Event = @event,
                                        MarketDetail = marketDetail,
                                        SportsbookRunner = sportsbookRunner,
                                        WinnerOddsString = winnerOddsString,
                                        ExpectedValue = expectedValueWin,
                                        ExchangeWinBestBlue = bestBlueWin!.Value,
                                        ExchangeWinBestPink = bestPinkWin!.Value,
                                        ExchangeWinBestPinkSize = bestPinkWinSize!.Value
                                    });
                                }

                                var rvm = new RunnerViewModel()
                                {
                                    SportsbookRunner = sportsbookRunner,
                                    ExchangeWinRunner = mappedExchangeWinRunner,
                                    SportsbookWinPrice = sportsbookRunner.winRunnerOdds.@decimal,
                                    ExpectedExchangeWinPrice = expectedWinPrice.Value,
                                    WinExpectedValue = expectedValueWin,
                                    WinnerOddsString = winnerOddsString
                                };

                                runners.Add(rvm);
                            }
                            catch (System.Exception exception)
                            {
                                var str = "";
                            }
                        }

                        var vm = new MarketViewModel(@event);
                        vm.SportsbookMarket = marketDetail;
                        vm.Runners = runners.OrderBy(r => r.SportsbookRunner.winRunnerOdds.@decimal);
                        vm.WinOverround = winOverround;

                        markets.Add(vm);
                    }
                }
                catch (System.Exception exception)
                {
                    var str = "";
                }
            }

            CatalogViewModel.Markets = markets;
            CatalogViewModel.BestWinRunners = bestWinRunners;

            return Page();
        }

        private static double ExpectedValue(double sportsbookPrice, double exchangePrice)
        {
            return ((sportsbookPrice - 1) * (1 / exchangePrice)) - ((1 - (1 / exchangePrice)));
        }

        private static double PlacePart(double sportsbookPrice, int denominator)
        {
            return denominator == 0 ? ((sportsbookPrice - 1) / 1) + 1 : ((sportsbookPrice - 1) / denominator) + 1;
        }
    }
}

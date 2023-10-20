using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages.Racing
{
    public class IndexModel : PageModel
    {
        private readonly IExchangeHandler _exchangeHandler;
        private readonly ISportsbookHandler _sportsbookHandler;
        private readonly ICatalogService _catalogService;
        private readonly IScrapingHandler _scrapingHandler;

        const string EventTypeId = "7";

        public CatalogViewModel CatalogViewModel { get; set; }

        [BindProperty]
        public RacingFormModel FormModel { get; set; }

        public IndexModel(IExchangeHandler exchangeHandler, ISportsbookHandler sportsbookHandler, ICatalogService catalogService, IScrapingHandler scrapingHandler)
        {
            _exchangeHandler = exchangeHandler;
            _sportsbookHandler = sportsbookHandler;
            _catalogService = catalogService;
            _scrapingHandler = scrapingHandler;

            CatalogViewModel = new CatalogViewModel();
        }

        public IActionResult OnGet(Bookmaker bookmaker = Bookmaker.Betfair)
        {
            var bestWinRunners = new List<BestRunner>();
            var bestEachWayRunners = new List<BestRunner>();

            try
            {
                var eventDict = _catalogService.GetExchangeEventsWithMarkets(EventTypeId);
                var marketCatalogues = _catalogService.GetExchangeMarketCatalogues(EventTypeId);
                var marketBooks = _catalogService.GetExchangeMarketBooks(marketCatalogues, eventDict);

                var sportsbookEventsWithMarkets = _catalogService.GetSportsbookEventsWithMarkets(EventTypeId);
                var sportsbookEventsWithPrices = _catalogService.GetSportsbookEventsWithPrices(sportsbookEventsWithMarkets);

                var markets = new List<MarketViewModel>();

                var scrapedEvents = _scrapingHandler.GetScrapedEvents(Domain.Enums.Bookmaker.Boylesports, DateTime.Today);

                foreach (var @event in sportsbookEventsWithPrices.Keys)
                {
                    try
                    {
                        var exchangeEventWithMarketBooks = marketBooks.FirstOrDefault(e => e.Key.Id == @event.Id);

                        var mappedEvent = sportsbookEventsWithPrices[@event];

                        if (mappedEvent == null) continue;

                        foreach (var marketDetail in mappedEvent)
                        {
                            var mappedScrapedEvent = scrapedEvents.FirstOrDefault(s =>
                                s.MappedEvent.Event.Venue == @event.Venue &&
                                s.MappedEvent.SportsbookMarket.marketStartTime == marketDetail.marketStartTime);

                            var exchangeMarketBooks = exchangeEventWithMarketBooks.Value
                                .FirstOrDefault(m => m.Key == marketDetail.marketStartTime);

                            if (exchangeMarketBooks.Value == null) continue;

                            var mappedWinMarketBook = exchangeMarketBooks.Value
                                .FirstOrDefault(m => m.NumberOfWinners == 1);

                            var mappedPlaceMarketBook = exchangeMarketBooks.Value
                                .FirstOrDefault(m => m.NumberOfWinners == marketDetail.numberOfPlaces);

                            var winOverround = marketDetail.runnerDetails
                                .Where(r => r.winRunnerOdds != null && r.winRunnerOdds.@decimal > 0 && r.runnerStatus == "ACTIVE")
                                .Sum(r => 1 / r.winRunnerOdds.@decimal) * 100;

                            var placeOverround = marketDetail.runnerDetails
                                .Where(r => r.winRunnerOdds != null && r.winRunnerOdds.@decimal > 0 && r.runnerStatus == "ACTIVE")
                                .Sum(r => (1 / (((r.winRunnerOdds.@decimal - 1) / marketDetail.placeFractionDenominator) + 1))) * 100;

                            var runners = new List<RunnerViewModel>();
                            foreach (var sportsbookRunner in marketDetail.runnerDetails)
                            {
                                try
                                {
                                    var mappedScrapedMarket = mappedScrapedEvent?.ScrapedMarkets
                                        .First();

                                    var mappedScrapedRunner = mappedScrapedMarket?.ScrapedRunners.FirstOrDefault(r =>
                                            r.Name.ToLower().Replace("'", "") == sportsbookRunner.selectionName.ToLower().Replace("'", ""));

                                    if (sportsbookRunner.runnerOrder >= 98 ||
                                        sportsbookRunner.runnerStatus != "ACTIVE" ||
                                        sportsbookRunner.winRunnerOdds == null)
                                    {
                                        continue;
                                    }

                                    var mappedExchangeWinRunner = mappedWinMarketBook?.Runners
                                        .FirstOrDefault(r => r.SelectionId == sportsbookRunner.selectionId);

                                    var mappedExchangePlaceRunner = mappedPlaceMarketBook?.Runners
                                        .FirstOrDefault(r => r.SelectionId == sportsbookRunner.selectionId);

                                    double? bestPinkWin = 0;
                                    double? bestPinkWinSize = 0;
                                    double? bestPinkPlace = 0;
                                    double? bestBlueWin = 0;
                                    double? bestBluePlace = 0;

                                    if (mappedExchangeWinRunner != null && mappedExchangePlaceRunner != null)
                                    {
                                        if (mappedExchangeWinRunner.ExchangePrices != null && mappedExchangePlaceRunner.ExchangePrices != null)
                                        {
                                            if (mappedExchangeWinRunner.ExchangePrices.AvailableToLay.Any() && mappedExchangePlaceRunner.ExchangePrices.AvailableToLay.Any())
                                            {
                                                bestPinkWin = mappedExchangeWinRunner.ExchangePrices.AvailableToLay[0]?.Price;
                                                bestPinkWinSize = mappedExchangeWinRunner.ExchangePrices.AvailableToLay[0]?.Size;
                                                bestPinkPlace = mappedExchangePlaceRunner.ExchangePrices.AvailableToLay[0]?.Price;
                                            }

                                            if (mappedExchangeWinRunner.ExchangePrices.AvailableToBack.Any() && mappedExchangePlaceRunner.ExchangePrices.AvailableToBack.Any())
                                            {
                                                bestBlueWin = mappedExchangeWinRunner.ExchangePrices.AvailableToBack[0]?.Price;
                                                bestBluePlace = mappedExchangePlaceRunner.ExchangePrices.AvailableToBack[0]?.Price;
                                            }
                                        }
                                    }

                                    var winnerOddsString = $"";
                                    var eachWayPlacePart = 1.00d;

                                    int numberOfPlaces = 1;
                                    int placeFraction = 1;

                                    double? winSpread = 0d;
                                    double? expectedWinPrice = 1d;

                                    double? placeSpread = 0d;
                                    double? expectedPlacePrice = 1d;

                                    double expectedValueWin = 1d;
                                    double expectedValuePlace = 1d;

                                    double eachWayExpectedValue = (expectedValueWin + expectedValuePlace) + 1;

                                    winSpread = bestPinkWin != null && bestPinkWin > 0 && bestBlueWin != null && bestBlueWin > 0 ? bestPinkWin - bestBlueWin : 0;
                                    expectedWinPrice = bestPinkWin != null && bestPinkWin > 0 && winSpread != null ? bestPinkWin - (winSpread * 0.1) : 1;

                                    placeSpread = bestPinkPlace != null && bestPinkPlace > 0 && bestBluePlace != null && bestBluePlace > 0 ? bestPinkPlace - bestBluePlace : 0;
                                    expectedPlacePrice = bestPinkPlace != null && bestPinkPlace > 0 && placeSpread != null ? bestPinkPlace - (placeSpread * 0.03) : 1;

                                    if (bookmaker == Bookmaker.Boylesports)
                                    {
                                        if (mappedScrapedRunner != null)
                                        {
                                            numberOfPlaces = mappedScrapedMarket!.ScrapedEachWayTerms.NumberOfPlaces;
                                            placeFraction = mappedScrapedMarket!.ScrapedEachWayTerms.EachWayFraction;

                                            winnerOddsString = $"{mappedScrapedRunner.ScrapedPrice.PriceString}";
                                            eachWayPlacePart = PlacePart((double)mappedScrapedRunner.ScrapedPrice.Decimal, placeFraction);

                                            expectedValueWin = ExpectedValue((double)mappedScrapedRunner.ScrapedPrice.Decimal, expectedWinPrice.Value);
                                            expectedValuePlace = ExpectedValue(eachWayPlacePart, expectedPlacePrice.Value);

                                            eachWayExpectedValue = (expectedValueWin + expectedValuePlace) + 1;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Could not map runner={sportsbookRunner.selectionName} in " +
                                                $"{marketDetail.marketName} {marketDetail.marketStartTime}");
                                        }
                                    }
                                    else
                                    {
                                        numberOfPlaces = marketDetail.numberOfPlaces;
                                        placeFraction = marketDetail.placeFractionDenominator;

                                        winnerOddsString = $"{sportsbookRunner.winRunnerOdds.numerator}/{sportsbookRunner.winRunnerOdds.denominator}";
                                        eachWayPlacePart = PlacePart(sportsbookRunner.winRunnerOdds.@decimal, marketDetail.placeFractionDenominator);

                                        expectedValueWin = ExpectedValue(sportsbookRunner.winRunnerOdds.@decimal, expectedWinPrice.Value);
                                        expectedValuePlace = ExpectedValue(eachWayPlacePart, expectedPlacePrice.Value);

                                        eachWayExpectedValue = (expectedValueWin + expectedValuePlace) + 1;
                                    }

                                    if (expectedValueWin > -0.03 && marketDetail.numberOfPlaces > 1 && expectedWinPrice > 1)
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
                                            ExchangeWinBestPinkSize = bestPinkWinSize!.Value,
                                            NumberOfPlaces = numberOfPlaces,
                                            PlaceFractionDenominator = placeFraction
                                        });
                                    }

                                    if (eachWayExpectedValue > 0.96 && marketDetail.numberOfPlaces > 1 && expectedWinPrice > 1)
                                    {
                                        bestEachWayRunners.Add(new BestRunner()
                                        {
                                            Event = @event,
                                            MarketDetail = marketDetail,
                                            SportsbookRunner = sportsbookRunner,
                                            WinnerOddsString = winnerOddsString,
                                            PlacePartOddsString = eachWayPlacePart.ToString("0.00"),
                                            ExpectedValue = eachWayExpectedValue,
                                            ExchangeWinBestPink = bestPinkWin!.Value,
                                            ExchangePlaceBestPink = bestPinkPlace!.Value,
                                            NumberOfPlaces = numberOfPlaces,
                                            PlaceFractionDenominator = placeFraction
                                        });
                                    }

                                    var rvm = new RunnerViewModel()
                                    {
                                        SportsbookRunner = sportsbookRunner,
                                        ExchangeWinRunner = mappedExchangeWinRunner,
                                        ExchangePlaceRunner = mappedExchangePlaceRunner,
                                        SportsbookWinPrice = sportsbookRunner.winRunnerOdds.@decimal,
                                        SportsbookPlacePrice = eachWayPlacePart,
                                        ExpectedExchangeWinPrice = expectedWinPrice.Value,
                                        ExpectedExchangePlacePrice = expectedPlacePrice.Value,
                                        EachWayExpectedValue = eachWayExpectedValue,
                                        WinExpectedValue = expectedValueWin,
                                        PlaceExpectedValue = expectedValuePlace,
                                        WinnerOddsString = winnerOddsString,
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
                            vm.EachWayPlaceOverround = placeOverround;

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
                CatalogViewModel.BestEachWayRunners = bestEachWayRunners;

                return Page();
            }
            catch (System.Exception exception)
            {
                return Page();
            }
        }

        public IActionResult OnPost(RacingFormModel formModel)
        {
            return OnGet(formModel.Bookmaker);
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

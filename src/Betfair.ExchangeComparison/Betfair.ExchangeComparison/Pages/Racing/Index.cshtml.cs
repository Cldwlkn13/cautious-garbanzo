using System.Collections.Concurrent;
using System.ComponentModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages.Racing
{
    public class IndexModel : PageModel
    {
        private readonly IExchangeHandler _exchangeHandler;
        private readonly ISportsbookHandler _sportsbookHandler;

        public CatalogViewModel CatalogViewModel { get; set; }

        public IndexModel(IExchangeHandler exchangeHandler, ISportsbookHandler sportsbookHandler)
        {
            _exchangeHandler = exchangeHandler;
            _sportsbookHandler = sportsbookHandler;

            CatalogViewModel = new CatalogViewModel();
        }

        public IActionResult OnGet()
        {
            var bestWinRunners = new List<BestRunner>();
            var bestEachWayRunners = new List<BestRunner>();

            try
            {
                if (!_exchangeHandler.SessionValid())
                {
                    var login = _exchangeHandler.Login("", "");
                }

                var marketCatalogues = _exchangeHandler.ListMarketCatalogues("7");

                var eventDict = new Dictionary<string, Event>();
                var eventsx = marketCatalogues.Select(m => m.Event);
                foreach (var @event in eventsx)
                {
                    if (!eventDict.ContainsKey(@event.Id))
                    {
                        eventDict.Add(@event.Id, @event);
                    }
                }

                var marketBooks = new Dictionary<Event, Dictionary<DateTime, IList<MarketBook>>>();
                foreach (var @event in marketCatalogues.GroupBy(m => m.Event.Id))
                {
                    var marketsInEvent = @event.GroupBy(m => m.Description.MarketTime).ToList();
                    var marketBooksInEvent = new Dictionary<DateTime, IList<MarketBook>>();

                    Parallel.ForEach(marketsInEvent, market =>
                    {
                        //foreach (var market in marketsInEvent)
                        //{
                        var marketIdsInRace = market.Select(m => m.MarketId);
                        var batchResult = _exchangeHandler.ListMarketBooks(marketIdsInRace.ToList());
                        marketBooksInEvent.Add(market.Key, batchResult);
                        //}
                    });

                    marketBooks.Add(eventDict[@event.Key], marketBooksInEvent);
                }

                var eventsWithMarkets = new ConcurrentDictionary<EventResult, IList<MarketCatalogue>>();
                var eventsWithPrices = new Dictionary<Event, IList<MarketDetail>>();

                if (!_sportsbookHandler.SessionValid())
                {
                    var sbklogin = _sportsbookHandler.Login("", "");
                }

                //var eventTypes = _sportsbookHandler.ListEventTypes();
                //var competitions = _sportsbookHandler.ListCompetitions();
                //var marketTypes = _sportsbookHandler.ListMarketTypes();

                var events = _sportsbookHandler.ListEventsByEventType("7");

                var eventIds = events.Where(e =>
                    e.Event.CountryCode == "GB" ||
                    e.Event.CountryCode == "IE")
                        .Select(e => e.Event.Id)
                        .ToHashSet();

                Parallel.ForEach(eventIds, eventId =>
                {
                    //foreach (var eventId in eventIds)
                    //{
                    var marketCatalogue = _sportsbookHandler.ListMarketCatalogues(new HashSet<string> { eventId });

                    eventsWithMarkets.AddOrUpdate(events.First(e => e.Event.Id == eventId), marketCatalogue, (k, v) => v = marketCatalogue);
                    //}
                });

                var marketIds = eventsWithMarkets.SelectMany(m => m.Value).Select(m => m.MarketId).ToList();

                var prices = _sportsbookHandler.ListPrices(marketIds);

                foreach (var eventResult in eventsWithMarkets)
                {
                    if (eventResult.Key.Event.Name.ToLower().Contains("odds") ||
                        eventResult.Key.Event.Name.ToLower().Contains("specials"))
                    {
                        continue;
                    }

                    var marketsInEvent = new List<MarketDetail>();

                    foreach (var marketCatalog in eventResult.Value)
                    {
                        var marketDetail = prices.marketDetails.FirstOrDefault(m => m.marketId == marketCatalog.MarketId);

                        if (marketDetail != null)
                        {
                            marketsInEvent.Add(marketDetail);
                        }
                    }

                    eventsWithPrices.Add(eventResult.Key.Event, marketsInEvent.OrderBy(m => m.marketStartTime).ToList());
                }
                //}

                var markets = new List<MarketViewModel>();

                foreach (var @event in eventsWithPrices.Keys)
                {
                    try
                    {
                        var exchangeEventWithMarketBooks = marketBooks.FirstOrDefault(e => e.Key.Id == @event.Id);

                        var mappedEvent = eventsWithPrices[@event];

                        if (mappedEvent == null) continue;

                        foreach (var marketDetail in mappedEvent)
                        {
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
                                                bestPinkPlace = mappedExchangePlaceRunner.ExchangePrices.AvailableToLay[0]?.Price;
                                            }

                                            if (mappedExchangeWinRunner.ExchangePrices.AvailableToBack.Any() && mappedExchangePlaceRunner.ExchangePrices.AvailableToBack.Any())
                                            {
                                                bestBlueWin = mappedExchangeWinRunner.ExchangePrices.AvailableToBack[0]?.Price;
                                                bestBluePlace = mappedExchangePlaceRunner.ExchangePrices.AvailableToBack[0]?.Price;
                                            }
                                        }
                                    }

                                    var winnerOddsString = $"{sportsbookRunner.winRunnerOdds.numerator}/{sportsbookRunner.winRunnerOdds.denominator}";
                                    var eachWayPlacePart = PlacePart(sportsbookRunner.winRunnerOdds.@decimal, marketDetail.placeFractionDenominator);

                                    var winSpread = bestPinkWin != null && bestPinkWin > 0 && bestBlueWin != null && bestBlueWin > 0 ? bestPinkWin - bestBlueWin : 0;
                                    var expectedWinPrice = bestPinkWin != null && bestPinkWin > 0 && winSpread != null ? bestPinkWin - (winSpread * 0.1) : 1;

                                    var placeSpread = bestPinkPlace != null && bestPinkPlace > 0 && bestBluePlace != null && bestBluePlace > 0 ? bestPinkPlace - bestBluePlace : 0;
                                    var expectedPlacePrice = bestPinkPlace != null && bestPinkPlace > 0 && placeSpread != null ? bestPinkPlace - (placeSpread * 0.1) : 1;

                                    var expectedValueWin = ExpectedValue(sportsbookRunner.winRunnerOdds.@decimal, expectedWinPrice.Value);
                                    var expectedValuePlace = ExpectedValue(eachWayPlacePart, expectedPlacePrice.Value);

                                    var eachWayExpectedValue = (expectedValueWin + expectedValuePlace) + 1;

                                    //if (sportsbookRunner.selectionName == "Cowboy Stuff")
                                    //{
                                    //    var str = "";
                                    //}

                                    if (expectedValueWin > -0.05 && marketDetail.numberOfPlaces > 1 && expectedWinPrice > 1)
                                    {
                                        bestWinRunners.Add(new BestRunner()
                                        {
                                            Event = @event,
                                            MarketDetail = marketDetail,
                                            SportsbookRunner = sportsbookRunner,
                                            WinnerOddsString = winnerOddsString,
                                            ExpectedValue = expectedValueWin,
                                            ExchangeWinBestPink = bestPinkWin!.Value
                                        });
                                    }

                                    if (eachWayExpectedValue > 0.93 && marketDetail.numberOfPlaces > 1 && expectedWinPrice > 1)
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
                                            ExchangePlaceBestPink = bestPinkPlace!.Value
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

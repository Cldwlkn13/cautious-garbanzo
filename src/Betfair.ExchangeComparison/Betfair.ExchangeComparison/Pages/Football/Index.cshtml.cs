using System.Collections.Concurrent;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages.Football
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

        public PageResult OnGet()
        {
            var bestWinRunners = new List<BestRunner>();

            if (!_exchangeHandler.SessionValid())
            {
                var login = _exchangeHandler.Login("", "");
            }

            var marketCatalogues = _exchangeHandler.ListMarketCatalogues("1");

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

            var eventsWithMarkets = new Dictionary<EventResult, IList<MarketCatalogue>>();
            var eventsWithPrices = new Dictionary<Event, IList<MarketDetail>>();

            if (!_sportsbookHandler.SessionValid())
            {
                var sbklogin = _sportsbookHandler.Login("", "");
            }

            //var eventTypes = _sportsbookHandler.ListEventTypes();
            //var competitions = _sportsbookHandler.ListCompetitions();
            //var marketTypes = _sportsbookHandler.ListMarketTypes();

            var events = _sportsbookHandler.ListEventsByEventType("1");

            var eventIds = events
                    .Select(e => e.Event.Id)
                    .ToHashSet();

            //Parallel.ForEach(eventIds, eventId =>
            //{
            foreach (var eventId in eventIds)
            {
                var marketCatalogue = _sportsbookHandler.ListMarketCatalogues(new HashSet<string> { eventId }, "1");

                eventsWithMarkets.Add(events.First(e => e.Event.Id == eventId), marketCatalogue);
            }
            //});

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

            var markets = new List<MarketViewModel>();

            foreach (var @event in eventsWithPrices.Keys)
            {
                try
                {
                    var exchangeEventWithMarketBooks = marketBooks.FirstOrDefault(e => e.Key.Id == @event.Id);

                    if (exchangeEventWithMarketBooks.Key == null)
                    {
                        continue;
                    }

                    var mappedEvent = eventsWithPrices[@event];

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

using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Trading.LimitAdjusters;
using Betfair.ExchangeComparison.Trading.StakeFilters;
using Betfair.ExchangeComparison.Trading.TradingFilters;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading
{
    public class TradingHandler : ITradingHandler
    {
        private readonly IOptions<TradingSettings> _settings;
        private readonly IMatchbookHandler _matchbookHandler;

        private readonly Dictionary<long, DateTime> SubmittedOfferIds;
        private readonly List<string> LoggingStore;
        
        public TradingHandler(IOptions<TradingSettings> settings, IMatchbookHandler matchbookHandler)
        {
            _settings = settings;
            _matchbookHandler = matchbookHandler;

            LoggingStore = new List<string>();
            SubmittedOfferIds = new Dictionary<long, DateTime>();
        }

        public async Task<CatalogViewModel> TradeCatalogue(CatalogViewModel cvm)
        {
            var offers = await TryGetOffers(cvm.AllMarketIds().ToArray());
            if(offers == null)
            {
                Console.WriteLine($"Could not retrieve Offers, exiting TradeCatalogue");
                return cvm;
            }

            UpdateSubmittedOfferIds(offers);

            foreach (var mvm in cvm.Markets)
            {
                if (mvm.MappedMatchbookEvent == null || mvm.MappedMatchbookEvent?.Id == default) continue;

                //get the win market
                var matchbookWinMarket = mvm.MappedMatchbookEvent.WinMarket();

                //check runner counts match across exchanges
                if(!PerformRunnerCountCheck(matchbookWinMarket, mvm)) 
                {
                    continue;
                }

                //extract offers on this market
                var marketOffers = MarketOffers(offers, matchbookWinMarket);

                //cancel any outdated offers
                await TryCancelOffers(marketOffers);

                //get runner and market positions
                var runnerPositions = GetRunnerPositions(mvm, marketOffers);
                var marketPosition = GetMarketPosition(mvm, matchbookWinMarket, runnerPositions);

                //go through each accumulated bestRunner
                foreach (var bestRunner in mvm.BestRunners)
                {
                    if (bestRunner.MappedMatchbookRunner == null ||
                        bestRunner.MappedMatchbookRunner?.Id == default) continue;

                    //get this runner position
                    var runnerPosition = marketPosition.RunnerPositions
                        .FirstOrDefault(r => r.MatchbookRunner.Id == bestRunner.MappedMatchbookRunner?.Id);

                    try
                    {
                        //can we trade this? 
                        if (!DetermineIfTrade(bestRunner, marketPosition))
                        {
                            continue;
                        }

                        //if we can trade determine the odds
                        if (!TryDetermineTradeOdds(bestRunner, out var tradeOdds))
                        {
                            continue;
                        }

                        //if we can trade determine the stake
                        if (!TryDetermineStake(bestRunner, marketPosition, runnerPosition, out var tradeStake))
                        {
                            continue;
                        }


                        //build the request
                        var offersRequest = TradingExtensions.NewOffersRequest(
                            bestRunner.MappedMatchbookRunner!.Id,
                            tradeStake,
                            tradeOdds, 
                            true);

                        try
                        {
                            //send the request
                            var offersResponse = await _matchbookHandler.PostOffer(offersRequest);

                            AddOrUpdateSubmittedOfferIds(offersResponse);

                            ConsoleExtensions.WriteLine($"Post Offer Request Successful: " +
                                $"{bestRunner.FiltersLogRunner()} " +
                                $"{tradeStake:F} @ {tradeOdds}", ConsoleColor.Blue, LoggingStore);

                            if (offersResponse!.Offers == null)
                                throw new NullReferenceException($"No Offers found in response.");

                            marketOffers.AddRange(offersResponse.Offers);
                        }
                        catch
                        {
                            Console.WriteLine($"Could not Post Offer, continuing...");
                            continue;
                        }
                    }
                    catch (System.Exception exception)
                    {
                        Console.WriteLine($"TRADING_HANDLER_EXCEPTION: {exception}");
                    }
                }

                //update current offers on the view model
                mvm.CurrentMarketOffers = marketOffers.Where(m => m.Side == "back").ToList();
            }

            offers = await TryGetOffers(cvm.AllMarketIds().ToArray());
            if (offers == null)
            {
                Console.WriteLine($"Could not retrieve Offers, exiting TradeCatalogue");
            }

            return cvm;
        }

        private bool DetermineIfTrade(BestRunner bestRunner, MyMarketPosition marketPosition)
        {
            var expectedPriceWithoutMargin = ((bestRunner.ExpectedPrice - 1) * 1.02) + 1;

            var runnerPosition = marketPosition.RunnerPositions
                .FirstOrDefault(r => r.MatchbookRunner.Id == bestRunner.MappedMatchbookRunner?.Id);

            if (runnerPosition == null) return false;

            //is the race type ok?
            //if (!RaceTypeFilters.FilterByRaceType(bestRunner, marketPosition, _settings, LoggingStore)) return false;
            //if (!RaceMetaFilters.FilterByRaceDistance(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //is the time to start within our window?
            if (!TimeFilters.FilterByTimeToRace(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //have we exceed max market laibility already?
            if (!MarketStateFilters.FilterByMarketOverround(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //have we exceed max market laibility already?
            if (!MarketLimitFilters.FilterByMaxMarketLiability(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //is the expected value for the pink within range?
            if (!PriceComparisonFilters.FilterByComparedExpectedValue(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //is there an appropriate amount requested on the pink?
            //if (!SizeComparisonFilters.FilterByPinkSizeAvailable(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //is the price below top price param?
            if (!PriceComparisonFilters.FilterByMaxPrice(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            //where do we stand with the runner position?
            if (!RunnerPositionFilters.FilterOnTradedPosition(bestRunner, marketPosition, runnerPosition, _settings, LoggingStore)) return false;

            //how many have we already backed in this race?
            //if (!MarketPositionFilters.FilterOnEntryOrder(bestRunner, marketPosition, runnerPosition, _settings, LoggingStore)) return false;

            //is the wap ok?
            if (!PriceComparisonFilters.FilterByWeightedAveragePrice(bestRunner, marketPosition, _settings, LoggingStore)) return false;

            return true;
        }

        private bool TryDetermineTradeOdds(BestRunner bestRunner, out float result)
        {
            if (!TryGetBestBackOddsAvailable(bestRunner, out var bestMatchbookBackOdds))
            {
                result = default;
                return false;
            }

            if (!TryGetBestLayOddsAvailable(bestRunner, out var bestMatchbookLayOdds))
            {
                result = default;
                return false;
            }

            var spread = Math.Abs(bestMatchbookLayOdds.Tick - bestMatchbookBackOdds.Tick);

            var closestTick = bestRunner.SportsbookRunner.winRunnerOdds.@decimal.GetClosestTick();
            var targetTick = bestRunner.SportsbookRunner.winRunnerOdds.@decimal != bestMatchbookBackOdds.Odds ?
                closestTick :
                bestMatchbookBackOdds.Tick;

            if (bestRunner.SportsbookRunner.winRunnerOdds.@decimal != bestMatchbookBackOdds.Odds && targetTick == bestMatchbookBackOdds.Tick)
            {
                ConsoleExtensions.WriteLine($"{nameof(TryDetermineTradeOdds)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Adjusting Target Tick + 1 " +
                    $"{bestRunner.SportsbookRunner.winRunnerOdds.@decimal} vs " +
                    $"{bestMatchbookBackOdds.Odds}", 
                    ConsoleColor.DarkYellow, LoggingStore);

                targetTick += 1;
            }

            if (bestMatchbookBackOdds.Tick > targetTick) //take best available
            {
                ConsoleExtensions.WriteLine($"{nameof(TryDetermineTradeOdds)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"BestMatchbookOdds {bestMatchbookBackOdds.Odds:F}[{bestMatchbookBackOdds.Tick}] " +
                    $"exceed Sbk Price {bestRunner.SportsbookRunner.winRunnerOdds.@decimal:F}[{targetTick}] " +
                    $"TargetPrice={bestMatchbookBackOdds.Odds:F}",
                        ConsoleColor.DarkYellow, LoggingStore);

                result = (float)bestMatchbookBackOdds.Odds;
                return true;
            }
            else if (bestMatchbookBackOdds.Tick == (targetTick - 1) && spread == 1 && bestRunner.ExchangeWinBestPinkSize > 50) //jump the spread
            {
                ConsoleExtensions.WriteLine($"{nameof(TryDetermineTradeOdds)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"BestMatchbookOdds {bestMatchbookBackOdds.Odds} are less than Target {MatchbookOddsLadder.TickToPrice[targetTick]} " +
                    $"Spread={spread} " +
                    $"BestPinkSize > 50 " +
                    $"Adjusting TargetPrice={bestMatchbookBackOdds.Odds:F}",
                        ConsoleColor.DarkYellow, LoggingStore);

                result = (float)MatchbookOddsLadder.TickToPrice[targetTick - 1];
                return true;
            }
            else //add at target price
            {
                result = (float)MatchbookOddsLadder.TickToPrice[targetTick];
                return true;
            }
        }

        private bool TryDetermineStake(BestRunner bestRunner, MyMarketPosition marketPosition, MyRunnerPosition runnerPosition, out float result)
        {
            result = 0;
            var targetWin = _settings.Value.TargetWinAmount;
            var maxRunnerLiability = _settings.Value.MaxRunnerLiability;

            //adjust params based on market state
            targetWin = bestRunner.AdjustTargetWinOnMarketVolumeTraded(
                targetWin, marketPosition, runnerPosition, _settings, LoggingStore);

            maxRunnerLiability = bestRunner.AdjustRunnerLiabilityOnMarketVolumeTraded(
                maxRunnerLiability, marketPosition, runnerPosition, _settings, LoggingStore);

            //how much left can we trade on runner?
            var fullAmountPossibleOnRunner = bestRunner.GetMaxPossibleOnRunner(
                targetWin, runnerPosition);

            // how much left can we trade on market?
            var fullAmountPossibleOnMarket = bestRunner.GetMaxPossibleOnMarket(
                _settings.Value.MaxRaceLiability, marketPosition);

            //whats our max based off the amount asked for?
            var maxFromPinkAvailable = bestRunner.GetMaxFromSizeAsked(
                _settings.Value.MaxAvailableStakeMultiplier, runnerPosition);

            result = (float)fullAmountPossibleOnRunner;
            result = (float)bestRunner.AdjustStakeOnRunnerLiability(fullAmountPossibleOnRunner, maxRunnerLiability, _settings, LoggingStore);
            //result = (float)bestRunner.AdjustStakeOnSizeAsked(result, maxFromPinkAvailable, _settings, LoggingStore);
            result = (float)bestRunner.AdjustStakeOnMarketLiability(result, fullAmountPossibleOnMarket, _settings, LoggingStore);

            return bestRunner.FinalizeStake(result, 0.5, _settings, LoggingStore);
        }

        private static bool TryGetBestBackOddsAvailable(BestRunner bestRunner, out Price result)
        {
            var matchbookPrices = bestRunner.MappedMatchbookRunner!.Prices
            .Where(p => p.Side == "back");

            var bestOddsAvailable = matchbookPrices
                .FirstOrDefault(p => p.Odds == matchbookPrices
                .Max(p => p.Odds));

            if (bestOddsAvailable == null)
            {
                result = new Price
                {
                    Odds = default
                };
                return false;
            }

            result = bestOddsAvailable;
            return true;
        }

        private static bool TryGetBestLayOddsAvailable(BestRunner bestRunner, out Price result)
        {
            var matchbookPrices = bestRunner.MappedMatchbookRunner!.Prices
            .Where(p => p.Side == "lay");

            var bestOddsAvailable = matchbookPrices
                .FirstOrDefault(p => p.Odds == matchbookPrices
                .Min(p => p.Odds));

            if (bestOddsAvailable == null)
            {
                result = new Price
                {
                    Odds = default
                };
                return false;
            }

            result = bestOddsAvailable;
            return true;
        }

        private static MyRunnerPosition ConvertFromRpo(RunnerPriceOverview rpo, IEnumerable<Offer> marketOffers)
        {
            var runnerPosition = new MyRunnerPosition();
            var offersForRunner = marketOffers
                .Where(o => o.RunnerId == rpo.MappedMatchbookRunner!.Id);
            var offersOtherRunners = marketOffers
                .Where(mb => mb.RunnerId != rpo.MappedMatchbookRunner!.Id);

            var matchedBetsForRunner = offersForRunner != null ?
                offersForRunner
                    .Where(o => o.MatchedBets != null)
                    .SelectMany(o => o.MatchedBets!).ToList() :
                        new List<MatchedBetInOffer>();

            var matchedBetsOtherRunners = offersOtherRunners != null ?
                 offersOtherRunners
                     .Where(o => o.MatchedBets != null)
                     .SelectMany(o => o.MatchedBets!).ToList() :
                         new List<MatchedBetInOffer>();

            var unmatchedStakesForRunner = offersForRunner != null ?
                offersForRunner
                    .Sum(o => o.Remaining!) : 0;

            var unmatchedPotentialProfitForRunner = offersForRunner != null ?
                offersForRunner
                    .Sum(o => o.RemainingPotentialProfit!) : 0;

            var unmatchedPriceForRunner = unmatchedStakesForRunner > 0 ?
                unmatchedPotentialProfitForRunner / unmatchedStakesForRunner : 0;

            runnerPosition.MatchedStakesOnOtherRunners = matchedBetsOtherRunners.Sum(mb => mb.Stake);
            runnerPosition.MatchbookRunner = rpo.MappedMatchbookRunner!;
            runnerPosition.MatchedStake = matchedBetsForRunner.Sum(mb => mb.Stake);
            runnerPosition.MatchedPotentialProfit = matchedBetsForRunner.Sum(mb => mb.PotentialProfit);
            runnerPosition.MatchedPrice = (runnerPosition.MatchedPotentialProfit / runnerPosition.MatchedStake) + 1;
            runnerPosition.MatchedBets = matchedBetsForRunner;
            runnerPosition.UnmatchedStake = unmatchedStakesForRunner;
            runnerPosition.UnmatchedPotentialProfit = unmatchedPotentialProfitForRunner;
            runnerPosition.UnmatchedPrice = unmatchedPriceForRunner;
            runnerPosition.Positon = runnerPosition.MatchedPotentialProfit - runnerPosition.MatchedStakesOnOtherRunners;
            runnerPosition.OpenBets = offersForRunner.Where(o => o.Status == "open").ToList();

            return runnerPosition;
        }

        private bool PerformRunnerCountCheck(MatchbookMarket matchbookWinMarket, MarketViewModel mvm)
        {
            try
            {
                var matchbookRunnerCount = matchbookWinMarket.Runners?
                    .Where(r => r.Status == "open")
                    .Count();
                var exchangeRunnerCount = mvm.Runners?
                    .Where(r => r.ExchangeWinRunner?.Status == Exchange.Model.RunnerStatus.ACTIVE)
                    .Count();

                if (matchbookRunnerCount == null || exchangeRunnerCount == null)
                {
                    return false;
                }

                if (matchbookRunnerCount != exchangeRunnerCount)
                {
                    var check = $"{mvm.Parent.Venue} " +
                        $"{mvm.SportsbookMarket.marketStartTime.ConvertUtcToBritishIrishLocalTime()} " +
                        $"Runner Counts do not match " +
                        $"Matchbook={matchbookRunnerCount} " +
                        $"Exchange={exchangeRunnerCount}";

                    if (!LoggingStore.Contains(check))
                    {
                        ConsoleExtensions.WriteLine(check, ConsoleColor.Red);
                    }

                    LoggingStore.Add(check);

                    return false;
                }

                return true;
            }
            catch(System.Exception exception)
            {
                Console.WriteLine();
                return false;
            }
        }

        private async Task<List<Offer>?> TryGetOffers(long[] marketIds)
        {
            try
            {
                var result = await _matchbookHandler.GetOffers(marketIds);
                //ConsoleExtensions.WriteLine($"{JsonConvert.SerializeObject(result)}",
                //    ConsoleColor.Green);
                return result;
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> TryCancelOffers(List<Offer> marketOffers)
        { 
            foreach(var offer in marketOffers)
            {
                var submittedOfferTime = SubmittedOfferIds.FirstOrDefault(o =>
                    o.Key == offer.RunnerId);

                if(submittedOfferTime.Value == default)
                {
                    continue;
                }

                if(offer.Remaining == offer.Stake && submittedOfferTime.Value.AddMinutes(30) < DateTime.Now)
                {
                    try
                    {
                        await _matchbookHandler.DeleteOffers(offer.RunnerId);
                        ConsoleExtensions.WriteLine($"Offers Cancelled for Runner {offer.RunnerName.CleanRunnerName()} at {DateTime.Now}",
                            ConsoleColor.Red,
                            LoggingStore);
                        if (SubmittedOfferIds.ContainsKey(offer.RunnerId))
                            SubmittedOfferIds.Remove(offer.RunnerId);
                    }
                    catch (System.Exception exception)
                    {
                        continue;
                    }
                }
            }

            return true;
        }

        private void UpdateSubmittedOfferIds(List<Offer> offers)
        {
            foreach (var offer in offers.Where(o => o.Status == "open"))
            {
                if (!SubmittedOfferIds.ContainsKey(offer.RunnerId))
                {
                    SubmittedOfferIds.Add(offer.RunnerId, DateTime.Now);
                }
            }
        }

        private void AddOrUpdateSubmittedOfferIds(OffersResponse? offersResponse)
        {
            if (offersResponse == null) return;

            foreach (var offer in offersResponse.Offers)
            {
                if (!SubmittedOfferIds.ContainsKey(offer.RunnerId))
                {
                    SubmittedOfferIds.Add(offer.RunnerId, DateTime.Now);
                }
                else
                {
                    SubmittedOfferIds[offer.RunnerId] = DateTime.Now;
                }
            }
        }

        private static List<Offer> MarketOffers(List<Offer> offers, MatchbookMarket matchbookMarket)
        {
            return offers
                    .Where(o => o.MarketId == matchbookMarket.Id)
                    .ToList();
        }

        private static List<MyRunnerPosition> GetRunnerPositions(MarketViewModel mvm, List<Offer> marketOffers)
        {
            var runnerPositions = new List<MyRunnerPosition>();
            foreach (var rpo in mvm.RunnerPriceOverviews)
            {
                if (rpo.MappedMatchbookRunner == null || rpo.MappedMatchbookRunner.Id == default) continue;

                runnerPositions.Add(ConvertFromRpo(rpo, marketOffers));
            }
            return runnerPositions;
        }

        private static MyMarketPosition GetMarketPosition(MarketViewModel mvm, MatchbookMarket matchbookWinMarket, List<MyRunnerPosition> runnerPositions)
        {
            double maxLoss = runnerPositions.Select(r => r.MatchedStakesOnOtherRunners).Max();
            return new MyMarketPosition()
            {
                EventName = mvm.MappedMatchbookEvent!.Name,
                MatchbookMarket = matchbookWinMarket,
                MaxLoss = maxLoss,
                RunnerPositions = runnerPositions
            };
        }
    }
}

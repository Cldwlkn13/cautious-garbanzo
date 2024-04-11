using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Settings;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Handlers
{
    public class TradingHandler : ITradingHandler
    {
        private readonly IOptions<TradingSettings> _settings;
        private readonly IMatchbookHandler _matchbookHandler;

        private List<Offer> AccountOffers;
        private DateTime LastRetrievedOffers; 
        
        public TradingHandler(IOptions<TradingSettings> settings, IMatchbookHandler matchbookHandler)
        {
            _settings = settings;
            _matchbookHandler = matchbookHandler;

            AccountOffers = new List<Offer>();
        }

        public async Task TradeCatalogue(CatalogViewModel cvm)
        {
            var allMarketIds = cvm.Markets
                .Where(m => m.MappedMatchbookEvent != null && m.MappedMatchbookEvent.Id != default)
                .Select(m => m.MappedMatchbookEvent.WinMarket().Id)
                .ToArray();

            List<Offer> offers = new List<Offer>();
            try
            {
                if(DateTime.Now > LastRetrievedOffers.AddMinutes(5))
                {
                    LastRetrievedOffers = DateTime.Now;
                    AccountOffers = await _matchbookHandler.GetOffers(allMarketIds);
                }
            }
            catch
            {
                Console.WriteLine($"Could not retrieve Offers, exiting TradeCatalogue()");
                return;
            }

            //foreach (var market in cvm.Markets.Where(m => m.Parent.Venue == "Wolverhampton"))
            foreach (var market in cvm.Markets)
                {
                if (market.MappedMatchbookEvent?.Id == default) continue;

                var winMarket = market.MappedMatchbookEvent.WinMarket();
                var marketOffers = offers.Where(o => o.MarketId == winMarket.Id);

                var runnerPositions = new List<MyRunnerPosition>();
                double maxLoss = 0;
                foreach(var rpo in market.RunnerPriceOverviews)
                {
                   if (rpo.MappedMatchbookRunner == null || rpo.MappedMatchbookRunner.Id == default) continue;
                    
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

                    var matchedStakesOtherRunners = matchedBetsOtherRunners.Sum(mb => mb.Stake);
                    maxLoss = matchedStakesOtherRunners > maxLoss ? -matchedStakesOtherRunners : maxLoss;

                    runnerPosition.MatchbookRunner = rpo.MappedMatchbookRunner!;
                    runnerPosition.MatchedStake = matchedBetsForRunner.Sum(mb => mb.Stake);
                    runnerPosition.MatchedPotentialProfit = matchedBetsForRunner.Sum(mb => mb.PotentialProfit);
                    runnerPosition.MatchedPrice = runnerPosition.MatchedPotentialProfit / runnerPosition.MatchedStake;
                    runnerPosition.MatchedBets = matchedBetsForRunner;
                    runnerPosition.UnmatchedStake = unmatchedStakesForRunner;
                    runnerPosition.UnmatchedPotentialProfit = unmatchedPotentialProfitForRunner;
                    runnerPosition.UnmatchedPrice = unmatchedPriceForRunner;
                    runnerPosition.Positon = runnerPosition.MatchedPotentialProfit - matchedStakesOtherRunners;
                    runnerPosition.OpenBets = offersForRunner.Where(o => o.Status == "open").ToList();

                    runnerPositions.Add(runnerPosition);
                }

                var marketPosition = new MyMarketPosition()
                {
                    EventName = market.MappedMatchbookEvent.Name,
                    MatchbookMarket = winMarket,
                    MaxLoss = maxLoss,
                    RunnerPositions = runnerPositions
                };

                foreach (var bestRunner in market.BestRunners)
                {
                    if (bestRunner.MappedMatchbookRunner == null ||
                        bestRunner.MappedMatchbookRunner?.Id == default) continue;

                    var runnerPosition = marketPosition.RunnerPositions
                        .FirstOrDefault(r => r.MatchbookRunner.Id == bestRunner.MappedMatchbookRunner?.Id);

                    if (DetermineIfTrade(bestRunner, marketPosition))
                    {
                        if (!TryDetermineTradeOdds(bestRunner, out var tradeOdds))
                        {
                            continue;
                        }

                        if(!TryDetermineStake(bestRunner, runnerPosition, out var tradeStake))
                        {
                            continue;
                        }

                        var offersRequest = NewRequest(
                            bestRunner.MappedMatchbookRunner!.Id, 
                            tradeStake, 
                            tradeOdds);

                        try
                        {
                            var offersResponse = await _matchbookHandler.PostOffer(offersRequest);

                            if (offersResponse!.Offers == null) 
                                throw new NullReferenceException($"No Offers found in response.");

                            foreach(var offer in offersResponse.Offers)
                            {
                                AccountOffers.Add(offer);
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"Could not Post Offer, continuing...");
                            continue;
                        }
                    }
                }           
            }
        }

        private bool DetermineIfTrade(BestRunner bestRunner, MyMarketPosition marketPosition)
        {
            var expectedPriceWithoutMargin = ((bestRunner.ExpectedPrice - 1) * 1.02) + 1;

            var runnerPosition = marketPosition.RunnerPositions
                .FirstOrDefault(r => r.MatchbookRunner.Id == bestRunner.MappedMatchbookRunner?.Id);

            if (runnerPosition == null) return false;

            if ((runnerPosition.MatchedStake + runnerPosition.UnmatchedStake) < _settings.Value.MaxRunnerLiability)
            if (bestRunner.SportsbookRunner.winRunnerOdds.@decimal >= expectedPriceWithoutMargin)
            if (bestRunner.ExpectedValueWin > -0.03)
            if (bestRunner.TimeToStart.Minutes < -45 && bestRunner.TimeToStart.Hours > -6)
            if (bestRunner.SportsbookRunner.winRunnerOdds.@decimal <= 31)
            {
                return true;            
            }

            return false;
        }

        private bool TryDetermineTradeOdds(BestRunner bestRunner, out float result)
        {
            if (!TryGetBestOddsAvailable(bestRunner, out var bestOdds))
            {
                result = default;
                return false;
            }

            var targetTick = bestRunner.SportsbookRunner.winRunnerOdds.@decimal.GetClosestTick();

            if (bestOdds.Tick >= (targetTick))
            {
                result = (float)bestOdds.Odds;
                return true;
            }
            else if (bestOdds.Tick == (targetTick - 1))
            {
                result = (float)MatchbookOddsLadder.TickToPrice[targetTick - 1];
                return true;
            }
            else
            {
                result = (float)MatchbookOddsLadder.TickToPrice[targetTick];
                return true;
            }
        }

        private bool TryDetermineStake(BestRunner bestRunner, MyRunnerPosition runnerPosition, out float result)
        {
            var targetWin = _settings.Value.TargetWinAmount;

            var availableStake = _settings.Value.MaxRunnerLiability - (runnerPosition.MatchedStake + runnerPosition.UnmatchedStake);

            var fullAmount = targetWin / (bestRunner.SportsbookRunner.winRunnerOdds.@decimal - 1);

            result = (float)fullAmount;
            if (fullAmount > _settings.Value.MaxRunnerLiability) 
            { 
                result = (float)_settings.Value.MaxRunnerLiability; 
            }
            if (result > availableStake)
            {
                result = (float)availableStake;
            }

            return result > 0.5 ? true : false;
        }

        private bool TryGetBestOddsAvailable(BestRunner bestRunner, out Price result)
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

        private OffersRequest NewRequest(long id, float stake, float odds)
        {
            var offerRequests = new List<OfferRequest> {
                                    new OfferRequest
                                    {
                                        RunnerId = id,
                                        Side = "back",
                                        Odds = odds,
                                        Stake = stake,
                                        KeepInPlay = false
                                    }
                                };
            return new OffersRequest
            {
                Offers = offerRequests
            };
        }
    }

    public class MyMarketPosition
    {
        public string EventName { get; set; }
        public MatchbookMarket MatchbookMarket { get; set; }
        public double MaxLoss { get; set; }
        public List<MyRunnerPosition> RunnerPositions { get; set; }

        public override string ToString()
        {
            return $"{EventName} - MaxLoss={MaxLoss.ToString("0.00")}"; 
        }
    }

    public class MyRunnerPosition
    {
        public MatchbookRunner MatchbookRunner { get; set; }

        public double MatchedStake { get; set; }
        public double MatchedPrice { get; set; }
        public double MatchedPotentialProfit { get; set; }
        public List<MatchedBetInOffer> MatchedBets { get; set; }

        public double UnmatchedStake { get; set; }
        public double UnmatchedPrice { get; set; }
        public double UnmatchedPotentialProfit { get; set; }
        public List<Offer> OpenBets { get; set; }

        public double Positon { get; set; }

        public override string ToString()
        {
            return $"{MatchbookRunner.Name} {Positon.ToString("0.00")}";
        }
    }
}

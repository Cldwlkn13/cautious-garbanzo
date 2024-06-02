using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.TradingModel
{
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
        public double MatchedStakesOnOtherRunners { get; set; }

        public override string ToString()
        {
            return $"{MatchbookRunner.Name} {Positon:F}";
        }
    }
}

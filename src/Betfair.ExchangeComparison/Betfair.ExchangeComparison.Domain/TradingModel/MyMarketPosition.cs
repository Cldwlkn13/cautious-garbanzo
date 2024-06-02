using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.TradingModel
{
    public class MyMarketPosition
    {
        public string EventName { get; set; }
        public MatchbookMarket MatchbookMarket { get; set; }
        public double MaxLoss { get; set; }
        public List<MyRunnerPosition> RunnerPositions { get; set; }

        public override string ToString()
        {
            return $"{EventName} - MaxLoss={MaxLoss:F}";
        }
    }
}

namespace Betfair.ExchangeComparison.Domain.Settings
{
    public class TradingSettings
    {
        public double MaxRaceLiability { get; set; }
        public double MaxRunnerLiability { get; set; }
        public double TargetWinAmount { get; set; }
        public int MaxAvailableStakeMultiplier { get; set; }
    }
}

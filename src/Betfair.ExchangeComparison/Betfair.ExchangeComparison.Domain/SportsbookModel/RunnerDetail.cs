namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class RunnerDetail
    {
        public int selectionId { get; set; }
        public string selectionName { get; set; }
        public int runnerOrder { get; set; }
        public WinRunnerOdds winRunnerOdds { get; set; }
        public EachwayRunnerOdds eachwayRunnerOdds { get; set; }
        public double handicap { get; set; }
        public string runnerStatus { get; set; }

        public override string ToString()
        {
            return $"{selectionName}";
        }
    }
}


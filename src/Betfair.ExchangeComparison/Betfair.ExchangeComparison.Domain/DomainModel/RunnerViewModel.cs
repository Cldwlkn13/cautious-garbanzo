using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Pages.Models
{
    public class RunnerViewModel
    {
        public RunnerDetail SportsbookRunner { get; set; }
        public Runner? ExchangeWinRunner { get; set; }
        public Runner? ExchangePlaceRunner { get; set; }
        public double SportsbookWinPrice { get; set; }
        public double SportsbookPlacePrice { get; set; }
        public double ExpectedExchangeWinPrice { get; set; }
        public double ExpectedExchangePlacePrice { get; set; }
        public double WinExpectedValue { get; set; }
        public double PlaceExpectedValue { get; set; }
        public double EachWayExpectedValue { get; set; }
        public string WinnerOddsString { get; set; }

        public RunnerViewModel()
        {
            SportsbookRunner = new RunnerDetail();
        }

        public override string ToString()
        {
            var name = SportsbookRunner != null && SportsbookRunner.selectionName != null ? SportsbookRunner.selectionName : "Unknown";
            var uuid = SportsbookRunner != null ? SportsbookRunner.selectionId.ToString() : "Unknown";

            return $"{name} {uuid} ";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (SportsbookRunner == null || SportsbookRunner.selectionName == null) return false;

            var runner = obj as RunnerViewModel;

            return runner!.SportsbookRunner.selectionName == SportsbookRunner.selectionName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}


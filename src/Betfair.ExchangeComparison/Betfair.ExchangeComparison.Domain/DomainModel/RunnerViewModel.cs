using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Matchbook;
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
        public MatchbookRunner MappedMatchbookRunner { get; set; }

        public RunnerViewModel()
        {
            SportsbookRunner = new RunnerDetail();
        }

        public RunnerViewModel(RunnerPriceOverview rpo)
        {
            SportsbookRunner = rpo.SportsbookRunner;
            SportsbookWinPrice = rpo.SportsbookRunner.winRunnerOdds.@decimal;
            ExpectedExchangeWinPrice = rpo.ExpectedWinPrice;
            ExpectedExchangePlacePrice = rpo.ExpectedPlacePrice;
            WinExpectedValue = rpo.ExpectedValueWin;
            PlaceExpectedValue = rpo.ExpectedValuePlace;
            EachWayExpectedValue = rpo.ExpectedValueEachWay;
            WinnerOddsString = rpo.WinnerOddsString;

            if(rpo.ExchangeWinRunner != null)
            {
                ExchangeWinRunner = rpo.ExchangeWinRunner;
            }

            if (rpo.ExchangePlaceRunner != null)
            {
                ExchangePlaceRunner = rpo.ExchangePlaceRunner;
            }
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


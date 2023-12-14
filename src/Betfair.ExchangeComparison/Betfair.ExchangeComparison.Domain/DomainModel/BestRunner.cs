using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Pages.Models
{
    public class BestRunner
    {
        public BestRunner()
        {

        }

        public BestRunner(RunnerPriceOverview rpo)
        {
            Sport = rpo.Sport;
            Competition = rpo.EventWithCompetition.Competition;
            Event = rpo.EventWithCompetition.Event;
            MarketDetail = rpo.MarketDetail;
            SportsbookRunner = rpo.SportsbookRunner;
            WinnerOddsString = rpo.WinnerOddsString;
            PlacePartOddsString = rpo.PlacePartOddsString;
            ExchangeWinBestBlue = rpo.BestWinAvailable[Side.BACK].Price;
            ExchangeWinBestPink = rpo.BestWinAvailable[Side.LAY].Price;
            ExchangeWinBestPinkSize = rpo.BestWinAvailable[Side.LAY].Size;
            ExchangePlaceBestPink = rpo.BestPlaceAvailable != null ? rpo.BestPlaceAvailable[Side.LAY].Price : 0;
            ExpectedValueWin = rpo.ExpectedValueWin;
            ExpectedValueEachWay = rpo.ExpectedValueEachWay;
            NumberOfPlaces = rpo.NumberEachWayPlaces;
            PlaceFractionDenominator = rpo.EachWayFraction;
            LastPriceTraded = rpo.LastPriceTradedWin;
            VolumeTradedBelowSportsbook = rpo.VolumeTradedBelowSportsbook;
            ExchangeWinBestPinkRequestedLiability = rpo.BestWinAvailable[Side.LAY].RequestedLiability();
            Bookmaker = rpo.Bookmaker;
            MappedEventName = rpo.MappedScrapedEventName;
            TimeToStart = rpo.MarketDetail.marketStartTime.TimeToStart();
        }

        public Sport Sport { get; set; }
        public Competition Competition { get; set; }
        public Event Event { get; set; }
        public MarketDetail MarketDetail { get; set; }
        public RunnerDetail SportsbookRunner { get; set; }
        public string WinnerOddsString { get; set; }
        public string PlacePartOddsString { get; set; }
        public double ExchangeWinBestBlue { get; set; }
        public double ExchangeWinBestPink { get; set; }
        public double ExchangeWinBestPinkSize { get; set; }
        public double ExchangeWinBestPinkRequestedLiability { get; set; }
        public double ExchangePlaceBestPink { get; set; }
        public double ExpectedValueWin { get; set; }
        public double ExpectedValueEachWay { get; set; }
        public double NumberOfPlaces { get; set; }
        public double PlaceFractionDenominator { get; set; }
        public double LastPriceTraded { get; set; }
        public double VolumeTradedBelowSportsbook { get; set; }
        public Bookmaker Bookmaker { get; set; }
        public string MappedEventName { get; set; }
        public TimeSpan TimeToStart { get; set; }
    }
}


using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Interfaces;
using Betfair.ExchangeComparison.Domain.Matchbook;
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
            MarketCatalogue = rpo.MarketCatalogue;
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
            MappedEventName = $"{rpo.EventWithCompetition.Event.Venue} {rpo.MarketDetail.marketStartTime.ConvertUtcToBritishIrishLocalTime()}";
            TimeToStart = rpo.MarketDetail.marketStartTime.TimeToStart();
            MappedMatchbookRunner = rpo.MappedMatchbookRunner;
            WeightedAveragePrice = rpo.WeightedAveragePrice;
            DifferenceToWeightedAveragePrice = rpo.SbkDifferenceToWeightedAveragePrice;
            TotalRunnerVolume = rpo.TotalRunnerVolume;
            TotalMarketVolume = rpo.TotalMarketVolume;
            MarketMeta = this.MapMarketMeta();

            if (rpo.MarketCatalogue.MarketName.Contains("Hrd") ||
                rpo.MarketCatalogue.MarketName.Contains("Chs") ||
                rpo.MarketCatalogue.MarketName.Contains("NHF"))
            {
                ModelParams = new JumpsParams(rpo);
            }
            else
            {
                ModelParams = new FlatParams(rpo);
            }
        }

        public Sport Sport { get; set; }
        public Competition Competition { get; set; }
        public Event Event { get; set; }
        public MarketDetail MarketDetail { get; set; }
        public MarketCatalogue MarketCatalogue { get; set; }
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
        public double WeightedAveragePrice { get; set; }
        public double DifferenceToWeightedAveragePrice { get; set; }
        public double TotalRunnerVolume { get; set; }
        public double TotalMarketVolume { get; set; }
        public Bookmaker Bookmaker { get; set; }
        public string MappedEventName { get; set; }
        public TimeSpan TimeToStart { get; set; }
        public double ExpectedPrice {  get; set; }
        public MatchbookRunner? MappedMatchbookRunner { get; set; }
        public IModelParams ModelParams { get; set; }
        public MarketMeta MarketMeta { get; set; }

        public override string ToString()
        {
            return $"{SportsbookRunner.selectionName}";
        }
    }
}


using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class RunnerPriceOverview
    {
        public Sport Sport { get; set; }
        public EventWithCompetition EventWithCompetition { get; set; }
        public MarketDetail MarketDetail { get; set; }
        public RunnerDetail SportsbookRunner { get; set; }
        public Runner ExchangeWinRunner { get; set; }
        public Runner ExchangePlaceRunner { get; set; }
        public Dictionary<Side, PriceSize> BestWinAvailable { get; set; }
        public Dictionary<Side, PriceSize> BestPlaceAvailable { get; set; }
        public string WinnerOddsString { get; set; }
        public string PlacePartOddsString { get; set; }
        public int NumberEachWayPlaces { get; set; }
        public int EachWayFraction { get; set; }
        public double EachWayPlacePart { get; set; }
        public double ExpectedWinPrice { get; set; }
        public double ExpectedPlacePrice { get; set; }
        public double ExpectedValueWin { get; set; }
        public double ExpectedValuePlace { get; set; }
        public double ExpectedValueEachWay { get; set; }
        public double LastPriceTradedWin { get; set; }
        public double LastPriceTradedPlace { get; set; }
        public double VolumeTradedBelowSportsbook { get; set; }
        public Bookmaker Bookmaker { get; set; }
        public string MappedScrapedEventName { get; set; }

        public RunnerPriceOverview()
        {
        }

        public RunnerPriceOverview(Sport sport, EventWithCompetition ewc, MarketDetail marketDetail, RunnerDetail sportsbookRunner, 
            Runner exchangeWinRunner, Runner? exchangePlaceRunner = null, 
            Bookmaker bookmaker = Bookmaker.BetfairSportsbook)
        {
            Sport = sport;
            EventWithCompetition = ewc;
            MarketDetail = marketDetail;
            SportsbookRunner = sportsbookRunner;
            Bookmaker = bookmaker;
            ExchangeWinRunner = exchangeWinRunner;
            BestWinAvailable = exchangeWinRunner.BestAvailable();
            ExpectedWinPrice = BestWinAvailable.ExpectedPrice(0);
            ExpectedValueWin = SportsbookRunner.winRunnerOdds.@decimal.ExpectedValue(ExpectedWinPrice);
            LastPriceTradedWin = exchangeWinRunner.LastPriceTraded();

            WinnerOddsString = new int[]
            {
                    SportsbookRunner.winRunnerOdds.numerator,
                    SportsbookRunner.winRunnerOdds.denominator
            }.OddsString();

            VolumeTradedBelowSportsbook = exchangeWinRunner.TradedVolumeBelowSportsbook(
                SportsbookRunner.winRunnerOdds.@decimal);

            NumberEachWayPlaces = marketDetail.numberOfPlaces;
            EachWayFraction = marketDetail.placeFractionDenominator;

            if (NumberEachWayPlaces > 0 && exchangePlaceRunner != null)
            {
                ExchangePlaceRunner = exchangePlaceRunner;
                BestPlaceAvailable = exchangePlaceRunner.BestAvailable();
                LastPriceTradedPlace = exchangePlaceRunner.LastPriceTraded();
                ExpectedPlacePrice = BestPlaceAvailable.ExpectedPrice(0);
                EachWayPlacePart = SportsbookRunner.winRunnerOdds.@decimal.PlacePart(EachWayFraction);
                PlacePartOddsString = EachWayPlacePart.ToString("0.00");
                ExpectedValuePlace = EachWayPlacePart.ExpectedValue(ExpectedPlacePrice);
                ExpectedValueEachWay = ((ExpectedValueWin + ExpectedValuePlace) / 2) + 1;
            }
        }

        public RunnerPriceOverview(Sport sport, EventWithCompetition ewc, MarketDetail marketDetail, ScrapedMarket scrapedMarket, 
            Runner exchangeWinRunner, RunnerDetail sportsbookRunner, ScrapedRunner scrapedRunner, Runner? exchangePlaceRunner = null, 
            Bookmaker bookmaker = Bookmaker.BetfairSportsbook, ScrapedEvent ? scrapedEvent = null)
        {
            Sport = sport;

            if (scrapedEvent != null)
            {
                MappedScrapedEventName = scrapedEvent.ScrapedEventName;
                ewc.Event.Name = scrapedEvent.BetfairName;
            }

            marketDetail.marketName = scrapedMarket.Name;

            EventWithCompetition = ewc;
            MarketDetail = marketDetail;
            Bookmaker = bookmaker;

            if (scrapedRunner.ScrapedPrices != null && 
                scrapedRunner.ScrapedPrices.TryGetScrapedPriceByBookmaker(bookmaker, out var scrapedPrice))
            {
                SportsbookRunner = new RunnerDetail
                {
                    selectionName = scrapedRunner.Name,
                    winRunnerOdds = new WinRunnerOdds
                    {
                        @decimal = (double)scrapedPrice.Decimal,
                        numerator = scrapedPrice.Numerator,
                        denominator = scrapedPrice.Denominator
                    }
                };
            }
            else
            {
                SportsbookRunner = sportsbookRunner;
            }

            ExchangeWinRunner = exchangeWinRunner;
            BestWinAvailable = exchangeWinRunner.BestAvailable();
            ExpectedWinPrice = BestWinAvailable.ExpectedPrice(0);
            ExpectedValueWin = SportsbookRunner.winRunnerOdds.@decimal.ExpectedValue(ExpectedWinPrice);
            LastPriceTradedWin = exchangeWinRunner.LastPriceTraded();

            WinnerOddsString = new int[]
            {
                    SportsbookRunner.winRunnerOdds.numerator,
                    SportsbookRunner.winRunnerOdds.denominator
            }.OddsString();

            VolumeTradedBelowSportsbook = exchangeWinRunner.TradedVolumeBelowSportsbook(
                SportsbookRunner.winRunnerOdds.@decimal);

            if (scrapedMarket.ScrapedEachWayTerms != null &&
                scrapedMarket.ScrapedEachWayTerms.TryGetScrapedEachWayTermsByBookmaker(
                    bookmaker, out var scrapedEachWayTerms))
            {
                NumberEachWayPlaces = scrapedEachWayTerms.NumberOfPlaces;
                EachWayFraction = scrapedEachWayTerms.EachWayFraction;
            }
            else
            {
                NumberEachWayPlaces = marketDetail.numberOfPlaces;
                EachWayFraction = marketDetail.placeFractionDenominator;
            }

            if (NumberEachWayPlaces > 0 && exchangePlaceRunner != null)
            {
                ExchangePlaceRunner = exchangePlaceRunner;        
                BestPlaceAvailable = exchangePlaceRunner.BestAvailable();
                LastPriceTradedPlace = exchangePlaceRunner.LastPriceTraded();
                ExpectedPlacePrice = BestPlaceAvailable.ExpectedPrice(0);
                EachWayPlacePart = SportsbookRunner.winRunnerOdds.@decimal.PlacePart(EachWayFraction);
                PlacePartOddsString = EachWayPlacePart.ToString("0.00");
                ExpectedValuePlace = EachWayPlacePart.ExpectedValue(ExpectedPlacePrice);
                ExpectedValueEachWay = (ExpectedValueWin + ExpectedValuePlace) + 1;
            }
        }


    }
}


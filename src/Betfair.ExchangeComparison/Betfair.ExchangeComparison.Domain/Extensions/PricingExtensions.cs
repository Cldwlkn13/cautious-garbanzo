using System;
using System.Drawing;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class PricingExtensions
    {
        public static Dictionary<Side, PriceSize> BestAvailable(this Runner runner)
        {
            var result = new Dictionary<Side, PriceSize>();

            if (runner.ExchangePrices != null)
            {
                if (runner.ExchangePrices.AvailableToLay.Any())
                {
                    result.Add(Side.LAY, runner.ExchangePrices.AvailableToLay[0]);
                }

                if (runner.ExchangePrices.AvailableToBack.Any())
                {
                    result.Add(Side.BACK, runner.ExchangePrices.AvailableToBack[0]);
                }
            }

            return result;
        }

        public static List<PriceSize> TradedVolume(this Runner runner)
        {
            if (runner.ExchangePrices != null)
            {
                if (runner.ExchangePrices.TradedVolume != null)
                {
                    return runner.ExchangePrices.TradedVolume;
                }
            }

            return new List<PriceSize>();
        }

        public static double LastPriceTraded(this Runner runner)
        {
            if (runner != null)
            {
                return runner.LastPriceTraded == null ? 0 : runner.LastPriceTraded.Value;
            }

            return 0;
        }

        public static PriceSize BestPriceSize(this Dictionary<Side, PriceSize> bestAvailable, Side side)
        {
            if (!bestAvailable.ContainsKey(side))
            {
                return new PriceSize { Price = 1.0, Size = 0 };
            }

            return bestAvailable[side];
        }

        public static double Spread(this Dictionary<Side, PriceSize> bestAvailable)
        {
            if (!bestAvailable.ContainsKey(Side.LAY) || !bestAvailable.ContainsKey(Side.BACK))
            {
                return default;
            }

            return bestAvailable[Side.LAY].Price - bestAvailable[Side.BACK].Price;
        }

        public static double ExpectedPrice(this Dictionary<Side, PriceSize> bestAvailable, double perc = 0)
        {
            var spread = bestAvailable.Spread();

            if (!bestAvailable.ContainsKey(Side.LAY))
            {
                return default;
            }

            return bestAvailable[Side.LAY] == null ? 0 : bestAvailable[Side.LAY].Price - (spread * perc);
        }

        public static double ExpectedValue(this double sportsbookPrice, double exchangePrice)
        {
            return ((sportsbookPrice - 1) * (1 / exchangePrice)) - ((1 - (1 / exchangePrice)));
        }

        public static double PlacePart(this double sportsbookPrice, int denominator)
        {
            return denominator == 0 ? ((sportsbookPrice - 1) / 1) + 1 : ((sportsbookPrice - 1) / denominator) + 1;
        }

        public static string OddsString(this decimal[] priceParts)
        {
            return $"{priceParts[0]}/{priceParts[1]}";
        }

        public static string OddsString(this double[] priceParts)
        {
            return $"{priceParts[0]}/{priceParts[1]}";
        }

        public static string OddsString(this int[] priceParts)
        {
            return $"{priceParts[0]}/{priceParts[1]}";
        }

        public static double WinOverround(this MarketDetail marketDetail)
        {
            return marketDetail.runnerDetails
                .Where(r => r.winRunnerOdds != null && r.winRunnerOdds.@decimal > 0 && r.runnerStatus == "ACTIVE")
                .Sum(r => 1 / r.winRunnerOdds.@decimal) * 100;
        }

        public static double PlaceOverround(this MarketDetail marketDetail)
        {
            return marketDetail.runnerDetails
                .Where(r => r.winRunnerOdds != null && r.winRunnerOdds.@decimal > 0 && r.runnerStatus == "ACTIVE")
                .Sum(r => (1 / (((r.winRunnerOdds.@decimal - 1) / marketDetail.placeFractionDenominator) + 1))) * 100;
        }

        public static double TradedVolumeBelowSportsbook(this Runner runner, double sportsbookPrice)
        {
            return runner.TradedVolume()
                 .Where(tv => tv.Price <= sportsbookPrice)
                 .Select(ps => ps.Price)
                 .Sum();
        }

        public static double RequestedLiability(this PriceSize priceSize)
        {
            return (priceSize.Price - 1) * priceSize.Size;
        }
    }
}


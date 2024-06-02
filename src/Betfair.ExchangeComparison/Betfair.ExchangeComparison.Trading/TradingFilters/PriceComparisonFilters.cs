using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class PriceComparisonFilters
    {
        public static bool FilterByComparedExpectedValue(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (bestRunner.ExpectedValueWin <= -0.015) //is the sbk ref price within expected value params vs best pink?
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByComparedExpectedValue)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"EV too low {bestRunner.ExpectedValueWin:F4}",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }
            return true;
        }

        public static bool FilterByWeightedAveragePrice(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (bestRunner.DifferenceToWeightedAveragePrice < 0.01) //is wap comparison ok?
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByWeightedAveragePrice)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Trading below WAP thereshold wap={bestRunner.WeightedAveragePrice:F} {bestRunner.DifferenceToWeightedAveragePrice:F4}",
                    bestRunner.DifferenceToWeightedAveragePrice >= 0 ? ConsoleColor.DarkCyan : ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }
            return true;
        }

        public static bool FilterByMaxPrice(this BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (bestRunner.SportsbookRunner.winRunnerOdds.@decimal > 31) //is the sbk ref price within max price threshold?
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByMaxPrice)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Price above top limit",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }
            return true;
        }
    }
}

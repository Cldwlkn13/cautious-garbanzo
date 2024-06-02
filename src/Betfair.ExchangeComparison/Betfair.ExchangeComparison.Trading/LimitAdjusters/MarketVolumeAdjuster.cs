using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.LimitAdjusters
{
    public static  class MarketVolumeAdjuster
    {
        public static double AdjustRunnerLiabilityOnMarketVolumeTraded(this BestRunner bestRunner, double currentValue, MyMarketPosition marketPosition, MyRunnerPosition runnerPosition, 
            IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            var result = currentValue;
            if (bestRunner.TotalMarketVolume < 5000)
            {
                double multiplier = 0.5;
                result = currentValue *= multiplier;

                ConsoleExtensions.WriteLine($"{nameof(AdjustRunnerLiabilityOnMarketVolumeTraded)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"TotalMarketVolume < 5000 Adjusting MaxRunnerLiability={result}",
                    ConsoleColor.DarkYellow, LoggingStore);
            }
            else if (bestRunner.TotalMarketVolume < 10000)
            {
                double multiplier = 0.75;
                result = currentValue *= multiplier;

                ConsoleExtensions.WriteLine($"{nameof(AdjustRunnerLiabilityOnMarketVolumeTraded)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"TotalMarketVolume < 10000 Adjusting MaxRunnerLiability={result}",
                    ConsoleColor.DarkYellow, LoggingStore);
            }
            return result;
        }

        public static double AdjustTargetWinOnMarketVolumeTraded(this BestRunner bestRunner, double currentValue, MyMarketPosition marketPosition, MyRunnerPosition runnerPosition,
            IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            var result = currentValue;
            if (bestRunner.TotalMarketVolume < 5000)
            {
                double multiplier = 0.5;
                result = currentValue *= multiplier;

                ConsoleExtensions.WriteLine($"{nameof(AdjustRunnerLiabilityOnMarketVolumeTraded)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"TotalMarketVolume < 5000 Adjusting TargetWin={result}",
                    ConsoleColor.DarkYellow, LoggingStore);
            }
            else if (bestRunner.TotalMarketVolume < 10000)
            {
                double multiplier = 0.75;
                result = currentValue *= multiplier;

                ConsoleExtensions.WriteLine($"{nameof(AdjustRunnerLiabilityOnMarketVolumeTraded)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"TotalMarketVolume < 10000 Adjusting TargetWin={result}",
                    ConsoleColor.DarkYellow, LoggingStore);
            }
            return result;
        }
    }
}

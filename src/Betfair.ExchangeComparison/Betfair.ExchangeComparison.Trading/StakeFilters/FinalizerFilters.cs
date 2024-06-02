using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.StakeFilters
{
    public static class FinalizerFilters
    {
        public static bool FinalizeStake(this BestRunner bestRunner, double currentValue, double thresholdValue,
            IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (currentValue <= thresholdValue)
            {
                ConsoleExtensions.WriteLine($"{nameof(FinalizeStake)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Final Stake {currentValue} too low. Return 0",
                    ConsoleColor.DarkYellow, null);

                return false;
            }
            return true;
        }
    }
}

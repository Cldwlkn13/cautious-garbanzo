using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.StakeFilters
{
    public static class MarketLiabilityFilters
    {
        public static double GetMaxPossibleOnMarket(this BestRunner bestRunner, double maxLiability, MyMarketPosition marketPosition)
        {
            return maxLiability - marketPosition.MaxLoss;
        }

        public static double AdjustStakeOnMarketLiability(this BestRunner bestRunner, double currentValue, double thresholdValue,
            IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (currentValue > thresholdValue) //make sure we aren't trying more than the max market limit
            {
                ConsoleExtensions.WriteLine($"{nameof(AdjustStakeOnMarketLiability)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Stake {currentValue} exceeds MarketLimit={thresholdValue:F} " +
                    $"Adjusting Stake={thresholdValue:F}",
                    ConsoleColor.DarkYellow, LoggingStore);

                return thresholdValue;
            }

            return currentValue;
        }
    }
}

using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.StakeFilters
{
    public static class SizeAskedFilters
    {
        public static double GetMaxFromSizeAsked(this BestRunner bestRunner, double multiplier, MyRunnerPosition runnerPosition)
        {
            return bestRunner.ExchangeWinBestPinkSize * multiplier;
                 //- (runnerPosition.MatchedStake + runnerPosition.UnmatchedStake);
        }

        public static double AdjustStakeOnSizeAsked(this BestRunner bestRunner, double currentValue, double thresholdValue,
                IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (currentValue > thresholdValue) //cap the amount staked at whats been put up * multiplier 
            {
                ConsoleExtensions.WriteLine($"{nameof(AdjustStakeOnSizeAsked)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Stake {currentValue} exceeds MaxAskedAtPink={thresholdValue:F} " +
                    $"Adjusting Stake={thresholdValue:F}",
                    ConsoleColor.DarkYellow, LoggingStore);

                return thresholdValue;
            }

            return currentValue;
        }
    }
}

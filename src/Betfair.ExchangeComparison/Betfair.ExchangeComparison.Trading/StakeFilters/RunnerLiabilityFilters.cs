using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.StakeFilters
{
    public static class RunnerLiabilityFilters
    {
        public static double GetMaxPossibleOnRunner(this BestRunner bestRunner, double targetWin, MyRunnerPosition runnerPosition)
        {
            return (targetWin / (bestRunner.SportsbookRunner.winRunnerOdds.@decimal - 1)) -
                (runnerPosition.MatchedStake + runnerPosition.UnmatchedStake);
        }

        public static double AdjustStakeOnRunnerLiability(this BestRunner bestRunner, double currentValue, double thresholdValue,
                IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (currentValue > thresholdValue) //make sure we aren't trying more than the max runner limit
            {
                ConsoleExtensions.WriteLine($"{nameof(AdjustStakeOnRunnerLiability)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Stake {currentValue:F} exceeds MaxRunnerLiability={thresholdValue:F} " +
                    $"Adjusting Stake={thresholdValue:F}",
                    ConsoleColor.DarkYellow, LoggingStore);

                return thresholdValue;
            }

            return currentValue;
        }
    }
}

using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class RunnerPositionFilters
    {
        public static bool FilterOnTradedPosition(BestRunner bestRunner, MyMarketPosition marketPosition, MyRunnerPosition runnerPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (runnerPosition.UnmatchedStake > 0 || runnerPosition.MatchedStake > 0)
            {
                if(bestRunner.SportsbookRunner.winRunnerOdds.@decimal < runnerPosition.MatchedPrice)
                {
                    ConsoleExtensions.WriteLine($"{nameof(FilterOnTradedPosition)} " +
                        $"{bestRunner.FiltersLogRunner()} " +
                        $"Price Already fixed at {runnerPosition.MatchedPrice:F}",
                        ConsoleColor.DarkMagenta, LoggingStore);
                    return false;
                }

                if(runnerPosition.MatchedStake + runnerPosition.UnmatchedStake > settings.Value.MaxRunnerLiability)
                {
                    ConsoleExtensions.WriteLine($"{nameof(FilterOnTradedPosition)} " +
                        $"{bestRunner.FiltersLogRunner()} " +
                        $"Max Runner Liability {settings.Value.MaxRunnerLiability:F} exceeded.",
                        ConsoleColor.DarkMagenta, LoggingStore);
                    return false;
                }
            }
            else
            {
                if (bestRunner.TimeToStart.TotalMinutes >= -10) // don't open a position on the show
                {
                    ConsoleExtensions.WriteLine($"{nameof(FilterOnTradedPosition)} " +
                        $"{bestRunner.FiltersLogRunner()} " +
                        $"Don't open position on the show",
                        ConsoleColor.DarkMagenta, LoggingStore);
                    return false;
                }
            }

            return true;
        }
    }
}

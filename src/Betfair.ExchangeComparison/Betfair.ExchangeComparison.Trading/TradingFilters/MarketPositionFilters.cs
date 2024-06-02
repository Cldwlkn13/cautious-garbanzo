using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class MarketPositionFilters
    {
        public static bool FilterOnEntryOrder(BestRunner bestRunner, MyMarketPosition marketPosition, MyRunnerPosition runnerPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            var otherRunnersWithBets = marketPosition.RunnerPositions
                .Where(x => bestRunner.MappedMatchbookRunner != null &&
                            x.MatchbookRunner.Name != bestRunner.MappedMatchbookRunner?.Name)
                .Where(x => x.MatchedBets.Count > 0 || x.OpenBets.Count > 0)
                .Count();

            if (otherRunnersWithBets >= 2)
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterOnEntryOrder)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"{otherRunnersWithBets} Runners have been backed",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }

            return true;
        }
    }
}

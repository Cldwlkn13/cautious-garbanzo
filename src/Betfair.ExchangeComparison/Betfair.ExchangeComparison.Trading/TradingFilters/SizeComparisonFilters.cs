using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class SizeComparisonFilters
    {
        public static bool FilterByPinkSizeAvailable(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (bestRunner.ExchangeWinBestPinkSize < 10) //is there an appropriate amount requested on the pink?
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByPinkSizeAvailable)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Not Enough asking at price {bestRunner.ExchangeWinBestPinkSize}",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }
            return true;
        }
    }
}

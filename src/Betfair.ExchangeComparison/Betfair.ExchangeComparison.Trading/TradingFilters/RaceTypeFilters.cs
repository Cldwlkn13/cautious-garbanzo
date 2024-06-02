using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class RaceTypeFilters
    {
        public static bool FilterByRaceType(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if (bestRunner.MarketMeta.IsJumps)
            {
                ConsoleExtensions.WriteLine($"{nameof(FilterByRaceType)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"IsJumps=true",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }

            return true;
        }
    }
}

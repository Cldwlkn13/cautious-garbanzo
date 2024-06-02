using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.TradingModel;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Trading.TradingFilters
{
    public static class TimeFilters
    {
        public static bool FilterByTimeToRace(BestRunner bestRunner, MyMarketPosition marketPosition, IOptions<TradingSettings> settings, List<string> LoggingStore)
        {
            if(bestRunner.TimeToStart.Hours <= -6)
            {
                ConsoleExtensions.WriteLine($"{nameof(TimeFilters)} " +
                    $"{bestRunner.FiltersLogRunner()} " +
                    $"Outside Time Window {bestRunner.TimeToStart.Hours}",
                    ConsoleColor.DarkMagenta, LoggingStore);
                return false;
            }
            return true;
        }
    }
}

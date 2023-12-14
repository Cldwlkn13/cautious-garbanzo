using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Pages.Models
{
    public class MarketViewModel
    {
        public Event Parent { get; set; }
        IEnumerable<MarketBook> ExchangeMarketBooks { get; set; }
        public MarketDetail SportsbookMarket { get; set; }
        public IEnumerable<RunnerViewModel> Runners { get; set; }
        public double WinOverround { get; set; }
        public double EachWayPlaceOverround { get; set; }
        public string ComparisonSource { get; set; }
        public IEnumerable<RunnerPriceOverview> RunnerPriceOverviews { get; set; }
        public IEnumerable<BestRunner> BestRunners { get; set; }
        public IEnumerable<BestRunner> BestEachWayRunners { get; set; }
        public TimeSpan TimeToStart { get; set; }

        public MarketViewModel(Event parent)
        {
            Parent = parent;
            SportsbookMarket = new MarketDetail();
            ExchangeMarketBooks = new List<MarketBook>();
            Runners = new List<RunnerViewModel>();
            RunnerPriceOverviews = new List<RunnerPriceOverview>();
            BestRunners = new List<BestRunner>();
            BestEachWayRunners = new List<BestRunner>();
        }

        public override string ToString()
        {
            var name = SportsbookMarket != null ? SportsbookMarket.marketName : "Unknown";
            var meeting = SportsbookMarket != null && Parent.Name != null ? Parent.Name : "Unknown";
            var uuid = SportsbookMarket != null ? SportsbookMarket.marketId : "Unknown";
            var offTime = SportsbookMarket != null ? SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToDateTimeString() : "Unknown";

            return $"{meeting} {offTime} - {name} {uuid}";
        }
    }

    public static class EventViewModelExtensions
    {
        public static string MinEventTimeString(this IEnumerable<MarketViewModel> eventViewModels)
        {
            return eventViewModels.MinEventTime().ToString("ddMM");
        }

        public static DateTime MinEventTime(this IEnumerable<MarketViewModel> eventViewModels)
        {
            return eventViewModels.Select(r => r.SportsbookMarket).Min(r => r.marketStartTime);
        }
    }
}


using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Pages.Model
{
    public class RacingFormModel
    {
        public Bookmaker Bookmaker { get; set; }
        public int RefreshRateSeconds { get; set; }
        public bool RefreshIsOn { get; set; }
    }
}


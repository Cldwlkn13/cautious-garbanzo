using System;
namespace Betfair.ExchangeComparison.Sportsbook.Settings
{
    public class SportsbookSettings
    {
        public string UrlBetfair { get; set; }
        public string UrlPaddyPower { get; set; }
        public bool UseBetfair { get; set; }

        public int RacingQueryToDays { get; set; }
        public int FootballQueryToHours { get; set; }
    }
}


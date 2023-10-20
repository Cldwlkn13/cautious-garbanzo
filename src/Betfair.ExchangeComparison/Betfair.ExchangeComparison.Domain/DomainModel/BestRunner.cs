﻿using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Pages.Models
{
    public class BestRunner
    {
        public Event Event { get; set; }
        public MarketDetail MarketDetail { get; set; }
        public RunnerDetail SportsbookRunner { get; set; }
        public string WinnerOddsString { get; set; }
        public string PlacePartOddsString { get; set; }
        public double ExchangeWinBestBlue { get; set; }
        public double ExchangeWinBestPink { get; set; }
        public double ExchangeWinBestPinkSize { get; set; }
        public double ExchangePlaceBestPink { get; set; }
        public double ExpectedValue { get; set; }
        public double NumberOfPlaces { get; set; }
        public double PlaceFractionDenominator { get; set; }
    }
}

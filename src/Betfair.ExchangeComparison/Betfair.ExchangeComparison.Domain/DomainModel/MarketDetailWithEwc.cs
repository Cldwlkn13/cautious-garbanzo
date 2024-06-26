﻿using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class MarketDetailWithEwc
    {
        public MarketDetailWithEwc()
        {
        }

        public EventWithCompetition EventWithCompetition { get; set; }
        public MarketDetail SportsbookMarket { get; set; }
    }
}


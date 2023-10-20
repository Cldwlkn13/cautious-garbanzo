using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class CompoundEventWithMarketDetail
    {
        public CompoundEventWithMarketDetail()
        {
        }

        public Event Event { get; set; }
        public MarketDetail SportsbookMarket { get; set; }
    }
}


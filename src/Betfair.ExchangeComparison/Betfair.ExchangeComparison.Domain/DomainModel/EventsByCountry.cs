using System;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class EventsByCountry
    {
        public EventsByCountry()
        {
        }

        public IEnumerable<IGrouping<string, IGrouping<EventByCountry, MarketCatalogue>>> Events { get; set; }
    }
}


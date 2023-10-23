using System;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class EventWithCompetition
    {
        public EventWithCompetition()
        {
        }

        public Competition Competition { get; set; }
        public Event Event { get; set; }
    }
}


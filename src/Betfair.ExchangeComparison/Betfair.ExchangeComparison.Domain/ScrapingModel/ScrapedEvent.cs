using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedEvent
    {
        public ScrapedEvent()
        {
            ScrapedMarkets = new List<ScrapedMarket>();
            ScrapedAt = DateTime.UtcNow;
        }

        public CompoundEventWithMarketDetail MappedEvent { get; set; }
        public string Name { get; set; }
        public List<ScrapedMarket> ScrapedMarkets { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ScrapedAt { get; set; }

        public override string ToString()
        {
            return $"{Name} - {StartTime}";
        }

        public override bool Equals(object? obj)
        {
            var comparer = obj as ScrapedEvent;
            return comparer?.Name == Name && comparer?.StartTime == StartTime;
        }
    }
}


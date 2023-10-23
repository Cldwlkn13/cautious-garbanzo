using Betfair.ExchangeComparison.Domain.DomainModel;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedEvent
    {
        public ScrapedEvent()
        {
            ScrapedMarkets = new List<ScrapedMarket>();
            ScrapedAt = DateTime.UtcNow;
        }

        public MarketDetailWithEvent MappedEvent { get; set; }
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


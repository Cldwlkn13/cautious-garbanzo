using Betfair.ExchangeComparison.Domain.DomainModel;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedEvent
    {
        public ScrapedEvent()
        {
            ScrapedMarkets = new List<ScrapedMarket>();
            ScrapedAt = DateTime.UtcNow;
            MappedEventWithCompetition = new EventWithCompetition();
        }

        public ScrapedEvent(IEnumerable<ScrapedMarket> markets)
        {
            ScrapedMarkets = markets.ToList();
            ScrapedAt = DateTime.UtcNow;
            MappedEventWithCompetition = new EventWithCompetition();
        }

        public EventWithCompetition MappedEventWithCompetition { get; set; }

        public string BetfairName { get; set; }
        public string ScrapedEventName { get; set; }
        public List<ScrapedMarket> ScrapedMarkets { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ScrapedAt { get; set; }

        public override string ToString()
        {
            return $"{BetfairName} - {StartTime}";
        }

        public override bool Equals(object? obj)
        {
            var comparer = obj as ScrapedEvent;
            return comparer?.BetfairName == BetfairName && comparer?.StartTime == StartTime;
        }
    }
}


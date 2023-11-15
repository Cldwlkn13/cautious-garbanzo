using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedMarket
    {
        public ScrapedMarket()
        {
            ScrapedRunners = new List<ScrapedRunner>();
            ScrapedEachWayTerms = new List<ScrapedEachWayTerms>();
            MappedMarketDetail = new MarketDetail();
        }

        public ScrapedMarket(string name)
        {
            Name = name;
            ScrapedRunners = new List<ScrapedRunner>();
            ScrapedEachWayTerms = new List<ScrapedEachWayTerms>();
            MappedMarketDetail = new MarketDetail();
        }

        public ScrapedMarket(string name, IEnumerable<ScrapedRunner> runners)
        {
            Name = name;
            ScrapedRunners = runners.ToList();
            ScrapedEachWayTerms = new List<ScrapedEachWayTerms>();
            MappedMarketDetail = new MarketDetail();
        }

        public MarketDetail MappedMarketDetail { get; set; }
        public string Name { get; set; }
        public List<ScrapedRunner> ScrapedRunners { get; set; }
        public List<ScrapedEachWayTerms> ScrapedEachWayTerms { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}


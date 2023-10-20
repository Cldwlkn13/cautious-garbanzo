using System;
namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedMarket
    {
        public ScrapedMarket()
        {
            ScrapedRunners = new List<ScrapedRunner>();
            ScrapedEachWayTerms = new ScrapedEachWayTerms();
        }

        public ScrapedMarket(string name)
        {
            Name = name;
            ScrapedRunners = new List<ScrapedRunner>();
            ScrapedEachWayTerms = new ScrapedEachWayTerms();
        }

        public string Name { get; set; }
        public List<ScrapedRunner> ScrapedRunners { get; set; }
        public ScrapedEachWayTerms ScrapedEachWayTerms { get; set; }

        public override string ToString()
        {
            return ScrapedEachWayTerms.NumberOfPlaces > 0 ? $"{Name} {ScrapedEachWayTerms.ToString()}" : $"{Name}";
        }
    }
}


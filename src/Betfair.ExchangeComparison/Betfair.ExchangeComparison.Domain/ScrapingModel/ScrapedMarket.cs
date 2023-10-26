using System;
using System.Collections.Generic;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedMarket
    {
        public ScrapedMarket()
        {
            ScrapedRunners = new List<ScrapedRunner>();
            ScrapedEachWayTerms = new List<ScrapedEachWayTerms>();
        }

        public ScrapedMarket(string name)
        {
            Name = name;
            ScrapedRunners = new List<ScrapedRunner>();
            ScrapedEachWayTerms = new List<ScrapedEachWayTerms>();
        }

        public string Name { get; set; }
        public List<ScrapedRunner> ScrapedRunners { get; set; }
        public List<ScrapedEachWayTerms> ScrapedEachWayTerms { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}


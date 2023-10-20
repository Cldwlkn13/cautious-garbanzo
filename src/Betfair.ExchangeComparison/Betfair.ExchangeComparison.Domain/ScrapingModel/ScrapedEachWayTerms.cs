using System;
namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedEachWayTerms
    {
        public ScrapedEachWayTerms()
        {
        }

        public ScrapedEachWayTerms(int places, int fraction)
        {
            NumberOfPlaces = places;
            EachWayFraction = fraction;
        }

        public int NumberOfPlaces { get; set; }
        public int EachWayFraction { get; set; }

        public override string ToString()
        {
            return $"{NumberOfPlaces} 1/{EachWayFraction}";
        }
    }
}


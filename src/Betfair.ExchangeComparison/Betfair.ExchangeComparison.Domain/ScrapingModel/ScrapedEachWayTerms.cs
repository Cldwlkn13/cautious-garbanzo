using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedEachWayTerms
    {
        public ScrapedEachWayTerms()
        {
        }

        public ScrapedEachWayTerms(int places, int fraction, Bookmaker bookmaker)
        {
            NumberOfPlaces = places;
            EachWayFraction = fraction;
            Bookmaker = bookmaker;
        }

        public Bookmaker Bookmaker { get; set; }
        public int NumberOfPlaces { get; set; }
        public int EachWayFraction { get; set; }

        public override string ToString()
        {
            return $"{Bookmaker} - {NumberOfPlaces} 1/{EachWayFraction}";
        }
    }
}


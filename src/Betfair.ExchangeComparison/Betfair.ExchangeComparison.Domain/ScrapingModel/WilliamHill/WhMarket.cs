using System;
using static System.Collections.Specialized.BitVector32;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.WilliamHill
{
    public class WhMarket
    {
        public bool active { get; set; }
        public string blurb { get; set; }
        public bool displayed { get; set; }
        public string id { get; set; }
        public string marketId { get; set; }
        public string marketName { get; set; }
        public string status { get; set; }
        public List<WhSelection> selections { get; set; }

        public override string ToString()
        {
            return $"{marketName}";
        }
    }
}


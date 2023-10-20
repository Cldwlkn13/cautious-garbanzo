using System;
using System.Xml.Linq;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedRunner
    {
        public ScrapedRunner()
        {
            ScrapedPrice = new ScrapedPrice();
        }

        public ScrapedRunner(string name, string priceString)
        {
            Name = name;
            ScrapedPrice = new ScrapedPrice(priceString);
        }

        public string Name { get; set; }
        public ScrapedPrice ScrapedPrice { get; set; }

        public override string ToString()
        {
            return $"{Name} - {ScrapedPrice.ToString()}";
        }
    }
}


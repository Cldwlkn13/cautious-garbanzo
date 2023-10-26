namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedRunner
    {
        public ScrapedRunner()
        {
            ScrapedPrices = new List<ScrapedPrice>();
        }

        public ScrapedRunner(string name)
        {
            Name = name;
            ScrapedPrices = new List<ScrapedPrice>();
        }

        public ScrapedRunner(string name, IEnumerable<ScrapedPrice> prices)
        {
            Name = name;
            ScrapedPrices = new List<ScrapedPrice>(prices);
        }

        public string Name { get; set; }
        public IEnumerable<ScrapedPrice> ScrapedPrices { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}


using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedRunner
    {
        public ScrapedRunner()
        {
            ScrapedPrices = new List<ScrapedPrice>();
            MappedRunnerDetail = new RunnerDetail();
        }

        public ScrapedRunner(string name)
        {
            Name = name;
            ScrapedPrices = new List<ScrapedPrice>();
            MappedRunnerDetail = new RunnerDetail();
        }

        public ScrapedRunner(string name, IEnumerable<ScrapedPrice> prices)
        {
            Name = name;
            ScrapedPrices = new List<ScrapedPrice>(prices);
            MappedRunnerDetail = new RunnerDetail();
        }

        public string Name { get; set; }
        public IEnumerable<ScrapedPrice> ScrapedPrices { get; set; }
        public RunnerDetail MappedRunnerDetail { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}


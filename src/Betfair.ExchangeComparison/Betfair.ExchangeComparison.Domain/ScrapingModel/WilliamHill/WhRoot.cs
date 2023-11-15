using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.WilliamHill
{
    public class WhRoot
    {
        public bool hasMoreItems { get; set; }
        public List<WhDatum> data { get; set; }
    }
}

